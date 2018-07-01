import Pages from "layouts/Pages.jsx";
import RTL from "layouts/RTL.jsx";
import Dashboard from "layouts/Dashboard.jsx";
import Visitor from "layouts/Visitor.jsx";

var indexRoutes = [
  { path: "/rtl", name: "RTL", component: RTL },
  { path: "/pages", name: "Pages", component: Pages },
  { path: "/Dashboard", name: "Home", component: Dashboard },
  { path: "/", name: "Visitor", component: Visitor }
];

export default indexRoutes;
