// import PricingPage from "views/Pages/PricingPage.jsx";
import LoginPage from "views/Pages/LoginPage.jsx";
import RegisterPage from "views/Pages/RegisterPage.jsx";
import LockScreenPage from "views/Pages/LockScreenPage.jsx";

// @material-ui/icons
import PersonAdd from "@material-ui/icons/PersonAdd";
import Fingerprint from "@material-ui/icons/Fingerprint";
// import MonetizationOn from "@material-ui/icons/MonetizationOn";
import LockOpen from "@material-ui/icons/LockOpen";

const visitorRoutes = [
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
  //   path: "/pages/pricing-page",
  //   name: "Pricing Page",
  //   short: "Pricing",
  //   mini: "PP",
  //   icon: MonetizationOn,
  //   component: PricingPage
  // },
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
