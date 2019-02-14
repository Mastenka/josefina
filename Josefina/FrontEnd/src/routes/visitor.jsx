import InfoPage from "views/Pages/InfoPage.jsx";
import LoginPage from "views/Pages/LoginPage.jsx";
import RegisterPage from "views/Pages/RegisterPage.jsx";

// @material-ui/icons
import PersonAdd from "@material-ui/icons/PersonAdd";
import Fingerprint from "@material-ui/icons/Fingerprint";
import Info from "@material-ui/icons/Info";

const visitorRoutes = [
   {
    path: "/visitor/info",
    name: "Info",
    short: "Info",
    mini: "PP",
    icon: Info,
    component: InfoPage
  },
  {
    path: "/visitor/register",
    name: "Registrace",
    short: "Registrace",
    mini: "RG",
    icon: PersonAdd,
    component: RegisterPage
  },
  {
    path: "/visitor/login",
    name: "Přihlášení",
    short: "Přihlášení",
    mini: "LG",
    icon: Fingerprint,
    component: LoginPage
  }
];

export default visitorRoutes;
