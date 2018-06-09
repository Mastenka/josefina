# Change Log

## [1.2.0] 2018-06-08
### **IMPORTANT!!!**
- All cards have been changed
- Please take a look in our [documentation about cards](https://demos.creative-tim.com/material-dashboard-pro-react/#/documentation/cards) and see how to make these changes
### Breaking changes
- Some props have been dropped, and some props have been added instead (please read our [live docs](https://demos.creative-tim.com/material-dashboard-pro-react/#/documentation/tutorial))
- Instead of using Material-UI's Cards, which are the base of our Cards in material-dashboard-pro-react@v1.1.1 and prior, we've decided to start from scratch and create our own components for the Cards
- All `*NoBackground` colors of the custom Button have been dropped, and have been replaced by adding the properties `simple color="*"` (where `*` is one of `info`, `danger` etc.)
- Dropped components
  - CustomButton
    - **`IconButton.jsx`** (instead of this one - use `Button` with `justIcon` prop)
  - Cards
    - **`ChartCard.jsx`**
    - **`PricingCard.jsx`**
    - **`FullHeaderCard.jsx`**
    - **`ProfileCard.jsx`**
    - **`HeaderCard.jsx`**
    - **`RegularCard.jsx`**
    - **`IconCard.jsx`**
    - **`StatsCard.jsx`**
    - **`ImagePriceCard.jsx`**
    - **`TasksCard.jsx`**
    - **`LoginCard.jsx`**
    - **`TestimonialCard.jsx`**
  - Grid
    - **`ItemGrid.jsx`**
- Added components
  - Cards (these are the replacement of the above deleted **Cards**)
    - **`Card.jsx`**
    - **`CardBody.jsx`**
    - **`CardHeader.jsx`**
    - **`CardFooter.jsx`**
    - **`CardIcon.jsx`**
  - **`CustomTabs/CustomTabs.jsx`** instead part of **`TasksCard.jsx`**
  - Grid
    - **`GridItem.jsx`** instead of the above deleted **Grid**
- Renamed the `*cardHeader` variables
  - from
    - `orangeCardHeader`
    - `greenCardHeader`
    - `redCardHeader`
    - `blueCardHeader`
    - `purpleCardHeader`
  - to
    - `warningCardHeader`
    - `successCardHeader`
    - `dangerCardHeader`
    - `infoCardHeader`
    - `primaryCardHeader`
- Changed the way we render `Switch`, `Select` and `Checkbox` (on Wizard page - Step2 - `IconCheckboxes`) components
- Changed the `ImageUpload` components - they weren't rendering correctly on apple products
- Major style changes:
  - `src/assets/jss/material-dashboard-pro-react/components/accordionStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/components/buttonStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/components/customInputStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/components/headerLinksStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/components/headerStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/components/navPillsStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/components/tasksStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/views/extendedTablesStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/views/notificationsStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/views/registerPageStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/views/userProfileStyles.jsx`
  - `src/assets/jss/material-dashboard-pro-react/views/validationFormsStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/customCheckboxRadioSwitch.jsx`
  - `src/assets/jss/material-dashboard-pro-react/customSelectStyle.jsx`
  - `src/assets/jss/material-dashboard-pro-react/modalStyle.jsx`
  - `src/assets/scss/material-dashboard-pro-react/plugins/_plugin-react-big-calendar.scss`
  - `src/assets/jss/material-dashboard-pro-react/components/customDropdownStyle.jsx`
  - `src/assets/scss/material-dashboard-pro-react/_fileupload.scss`
### Bug Fixing
- Due to the change of material-ui, all the imports from this library have been changed
- Added props on the `Wizard` component and **Wizard Steps** components so that you can pass states between them ([please read the docs](https://demos.creative-tim.com/material-dashboard-pro-react/#/documentation/wizard))
- Used prettier to make the code more readable
- Added `/*eslint-disable*/` at the start of some files to stop showing warnings about links
### Deleted dependencies
- `material-ui@1.0.0-beta.41`
### Added dependencies
- `@material-ui/core@1.2.0` (instead of `material-ui@1.0.0-beta.41`)
- `ajv@6.5.0` to stop the warning `npm **WARN** ajv-keywords@3.2.0 requires a peer of ajv@^6.0.0 but none is installed. You must install peer dependencies yourself.`
- `@types/markerclustererplus@2.1.33` to stop the warning `npm **WARN** react-google-maps@9.4.5 requires a peer of @types/markerclustererplus@^2.1.29 but none is installed. You must install peer dependencies yourself.`
- `@types/googlemaps@3.30.8` to stop the warning `npm **WARN** react-google-maps@9.4.5 requires a peer of @types/googlemaps@^3.0.0 but none is installed. You must install peer dependencies yourself.`
### Updated dependencies
- `@material-ui/icons@1.0.0-beta.42` to `@material-ui/icons@1.1.0`
- `node-sass-chokidar@1.2.2` to `node-sass-chokidar@1.3.0`
- `npm-run-all@4.1.2` to `npm-run-all@4.1.3`
- `react@16.2.0` to `react@16.4.0`
- `react-big-calendar@0.18.0` to `react-big-calendar@0.19.1`
- `react-bootstrap-sweetalert@4.2.3` to `react-bootstrap-sweetalert@4.4.1`
- `react-dom@16.2.0` to `react-dom@16.4.0`
- `react-jvectormap@0.0.2` to `react-jvectormap@0.0.3`
- `react-table@6.8.0` to `react-table@6.8.6`

## [1.1.1] 2018-05-22
### Bug Fixing
- Changed links for live preview, online documentation and issues
- Changed links from `http` to `https`

## [1.1.0] 2018-04-16
### Bug Fixing
- Changes caused by the upgrade of `material-ui`
### Deleted dependencies
- `material-ui-icons@1.0.0-beta.36`
### Added dependencies
- `@material-ui/icons@1.0.0-beta.42` (instead of `material-ui-icons@1.0.0-beta.36`)
### Updated dependencies
- `material-ui@1.0.0-beta.34` to `material-ui@1.0.0-beta.41`
- `npm-run-all@4.1.1` to `npm-run-all@4.1.2`
- `react-scripts@1.1.1` to `react-scripts@1.1.4`
- `node-sass-chokidar@0.0.3` to `node-sass-chokidar@1.2.2`
- `moment@2.21.0` to `moment@2.22.1`

## [1.0.0] 2018-03-27
### Original Release
- Added Material-UI as base framework
- Added design from Material Dashboard Pro BS3 by Creative Tim
