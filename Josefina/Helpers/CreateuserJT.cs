using System;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using Josefina.Mailers;

namespace Josefina
{
    class UserJT
    {
        private string apiUrl, apiLogin, apiPassword;

        /**
            Inicializace nastaveni API a emailu
         */
        public UserJT()
        {
            // api nastaveni
            this.apiUrl = "http://junktown.eu/wp-json/wp/v2";
            this.apiLogin = "rest_api";
            this.apiPassword = "&*UhI(99WYqLWgfiyZSJIYJ4";
 }

        /**
            Vytvori uzivatele dle zadaneho emailu, vygeneruje mu nahodne heslo a zasle informacni email
         */
        public WpUser createUserInWordpress(string email)
        {
            // navratova hodnota
            var user = new WpUser();
            // vygeneruj heslo
            var password = this.generatePassword(10);
            // email a heslo jsou pouzitelne
            if (this.isValidEmail(email) && !String.IsNullOrEmpty(password))
            {
                // vytvor wp ucet
                try
                {
                    user = this.SendRESTrequest(email, password);
                    user.email = email;
                }
                catch (Exception ex)
                {
                    user.code = "create_account_fail";
                    user.email = email;
                    user.message = String.Format("Nepodarilo se vytvorit ucet {0} ve wordpressu!", email);
                    user.data = ex.ToString();
                }
                // posli email s heslem
                if (user.isValidUser())
                {
                    try
                    {
                        IUserMailer userMailer = new UserMailer();
                        var emailer = userMailer.SendUserCreatedJT(email, password);
                        emailer.Send();
                    }
                    catch (Exception ex)
                    {
                        user.code = "send_email_fail";
                        user.email = email;
                        user.message = String.Format("Nepodarilo se odeslat email uzivateli '{0}' s heslem '{1}'!", user.email, user.password);
                        user.data = ex.ToString();
                    }
                }
            }
            else
            {
                // pridej chybovou hlasku
                user.code = "bad_email_or_password";
                user.email = email;
                user.message = "Chybne zadany email nebo spatne vygenerovane heslo!";
            }

            return user;
        }

        /**
            Odesle REST pozadavek na vytvoreni uzivatele ve Wordpressu
         */
        private WpUser SendRESTrequest(string email, string password)
        {
            var client = new RestClient(this.apiUrl);
            // wordpress api prihlaseni uctem s admin pravy
            client.Authenticator = new HttpBasicAuthenticator(this.apiLogin, this.apiPassword);
            // vytvoreni requestu s parametry noveho uzivatele
            var request = new RestRequest("users", Method.POST);
            request.AddParameter("username", email);
            request.AddParameter("email", email);
            request.AddParameter("password", password);
            // odeslani pozadavku do wordpressu
            IRestResponse response = client.Execute(request);
            // preved odpoved na object WpUser
            RestSharp.Deserializers.JsonDeserializer deserial = new JsonDeserializer();
            var user = deserial.Deserialize<WpUser>(response);
            user.password = password;

            return user;
        }

        /**
            Vrati nahodne heslo dle zadane delky
         */
        private string generatePassword(int length)
        {
            var result = Guid.NewGuid().ToString("N").ToLower().Replace("1", "").Replace("o", "").Replace("0", "");
            length = length < result.Length ? length : result.Length;
            return Guid.NewGuid().ToString("N").ToLower().Replace("1", "").Replace("o", "").Replace("0", "").Substring(0, length);
        }

        /**
            Overi zda je email validni
         */
        private bool isValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public class WpUser
        {
            public string id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            // kod chyby
            public string code { get; set; }
            // chybova hlaska
            public string message { get; set; }
            // doplnujici data chyby
            public string data { get; set; }
            public string password { get; set; }

            /**
                Vraci informaci zda existuje ID uzivatele a zda neobsahuje chybovy kod
             */
            public bool isValidUser()
            {
                return !String.IsNullOrEmpty(this.id) && String.IsNullOrEmpty(this.code);
            }
        }

        public void testRead()
        {
            var client = new RestClient(this.apiUrl);
            client.Authenticator = new HttpBasicAuthenticator(this.apiLogin, this.apiPassword);

            var request = new RestRequest("users/1", Method.GET);
            request.RequestFormat = DataFormat.Json;

            IRestResponse response = client.Execute(request);

            Console.WriteLine("API response:");
            Console.WriteLine(response.Content);
            Console.WriteLine();

            RestSharp.Deserializers.JsonDeserializer deserial = new JsonDeserializer();
            var user = deserial.Deserialize<WpUser>(response);

            if (user.isValidUser())
            {
                Console.WriteLine("ID: " + user.id);
                Console.WriteLine("NAME: " + user.name);
                Console.WriteLine("EMAIL: " + user.email);
            }
            else
            {
                Console.WriteLine("CODE: " + user.code);
                Console.WriteLine("MSG: " + user.message);
                Console.WriteLine("DATA: " + user.data);
            }
        }
    }
}
