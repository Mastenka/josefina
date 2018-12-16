import React from "react";
import PropTypes from "prop-types";
import { Switch, Route, Redirect } from "react-router-dom";

// @material-ui/core components
import withStyles from "@material-ui/core/styles/withStyles";

// core components
import VisitorHeader from "components/Header/VisitorHeader.jsx";
import Footer from "components/Footer/Footer.jsx";

import VisitorRoutes from "routes/visitor.jsx";

import pagesStyle from "assets/jss/material-dashboard-pro-react/layouts/pagesStyle.jsx";
    
import bgImage from "assets/img/register.jpeg";

// var ps;

class Visitor extends React.Component {

  componentDidMount() {
    document.body.style.overflow = "unset";
  } 

  render() {
    const { classes, ...rest } = this.props;
    return (
      <div>
        <VisitorHeader {...rest} />
        <div className={classes.wrapper} ref="wrapper">
          <div
            className={classes.fullPage}
            style={{ backgroundImage: "url(" + bgImage + ")" }}
          >
            <Switch>
              {VisitorRoutes.map((prop, key) => {
                if (prop.collapse) {
                  return null;
                }
                if (prop.redirect) {
                  return (
                    <Redirect from={prop.path} to={prop.pathTo} key={key} />
                  );
                }
                return (
                  <Route
                    path={prop.path}
                    component={prop.component}
                    key={key}
                  />
                );
              })}
            </Switch>
            <Footer white />
          </div>
        </div>
      </div>
    );
  }
}

Visitor.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(pagesStyle)(Visitor);
