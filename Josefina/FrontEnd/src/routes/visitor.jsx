import InfoPage from "views/Pages/InfoPage.jsx";
import LoginPage from "views/Pages/LoginPage.jsx";
import RegisterPage from "views/Pages/RegisterPage.jsx";
import LockScreenPage from "views/Pages/LockScreenPage.jsx";

// @material-ui/icons
import PersonAdd from "@material-ui/icons/PersonAdd";
import Fingerprint from "@material-ui/icons/Fingerprint";
import Info from "@material-ui/icons/Info";
import LockOpen from "@material-ui/icons/LockOpen";

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
  },
  // {
  //   path: "/pages/lock-screen-page",
  //   name: "Lock Screen Page",
  //   short: "Lock",
  //   mini: "LSP",
  //   icon: LockOpen,
  //   component: LockScreenPage
  // },
  // {
  //   redirect: true,
  //   path: "/pages",
  //   pathTo: "/pages/register-page",
  //   name: "Register Page"
  // }
];

export default visitorRoutes;
