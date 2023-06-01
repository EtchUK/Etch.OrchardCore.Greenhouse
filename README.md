# Etch.OrchardCore.Greenhouse

Integrates Greenhouse with Orchard Core

## Build Status

[![NuGet](https://img.shields.io/nuget/v/Etch.OrchardCore.Greenhouse.svg)](https://www.nuget.org/packages/Etch.OrchardCore.Greenhouse)

## Orchard Core Reference

This module is referencing a stable build of Orchard Core ([`1.5.0`](https://www.nuget.org/packages/OrchardCore.Module.Targets/1.5.0)).

## Installing

This module is available on [NuGet](https://www.nuget.org/packages/Etch.OrchardCore.Greenhouse). Add a reference to your Orchard Core web project via the NuGet package manager. Search for "Etch.OrchardCore.Greenhouse", ensuring include prereleases is checked.

Alternatively you can [download the source](https://github.com/etchuk/Etch.OrchardCore.Greenhouse/archive/main.zip) or clone the repository to your local machine. Add the project to your solution that contains an Orchard Core project and add a reference to Etch.OrchardCore.Greenhouse.

## Usage

First step is to enable "Greenhouse" within the features section of the admin dashboard. Enabling the module will make a new "Greenhouse" option available within the "Configuration" section of the admin menu. This option will navigate to the settings for this module, which is where settings need to be configured in order for the site to communicate with the Greenhouse API.

### Fetching Postings

Enabling this module will create a new "Greenhouse Posting" content type that will represent a posting within Greenhouse. Workflows are leveraged in order to sync job postings from Greenhouse. By using workflows it gives you control of the trigger for when a sync should be initiated.

#### Sync Greenhouse Board

When creating a workflow, there is a "Sync Greenhouse Board" task that contains some optional configuration options. The sync will fetch jobs from a job board whose token is specified within the task.

### Applying

The "Greenhouse Posting" content type contains a "GreenhousePostingFormPart" that can be used to control whether the application form is displayed and various settings for configuring the behaviour of the application form. The application form fields are represented as JSON and stored within the `GreenhousePostingFormPart`. Application forms can be made up of different types of fields (e.g. short text, multi select, etc...). When rendering the `GreenhousePostingFormPart`, each field within the application form is represented by a shape that's is then rendered by the `GreenhousePostingFormPart` shape. Theme developers can override these shapes with a custom template. For example to customise the long text field display, add a `GreenhouseQuestion-LongText` template to your theme.

The currently supported form fields are shown below.

-   Attachment
-   Boolean
-   LongText
-   MultiSelect
-   ShortText
-   SingleSelect

#### Workflows

This module provides a "Greenhouse application event" that will trigger when an application is submitted through the site. There are outcomes for handling whether the application was successful or failed.

### Listings

This module provides a "Greenhouse Postings" widget that provides a variety of ways to display a collection of greenhouse postings. This widget includes options for providing basic filters for filtering posts by location or department, paging, customising layout and defining labels displayed on the UI.

### Liquid Filters

Below are the liquid flters that are provided.

#### greenhouse_department_options

Returns a collection of `<option>` elements that represent a distinct list of the different departments associated to greenhouse postings within site. This filter needs the results from an Orchard Core query ("AllGreenhousePostings" is created when enabling this module) that contains all the Greenhouse postings in order to access the departments. If a `department` query string is present and has a matching value then the `<option>` element will have a `selected` attribute. Below is an example of how to use the filter.

```
{% assign allPostings = Queries.AllGreenhousePostings | query %}
{{ allPostings | greenhouse_department_options | raw }}
```

#### greenhouse_location_options

Returns a collection of `<option>` elements that represent a distinct list of the different locations associated to greenhouse postings within site. This filter needs the results from an Orchard Core query ("AllGreenhousePostings" is created when enabling this module) that contains all the Greenhouse postings in order to access the locations. If a `location` query string is present and has a matching value then the `<option>` element will have a `selected` attribute. Below is an example of how to use the filter.

```
{% assign allPostings = Queries.AllGreenhousePostings | query %}
{{ allPostings | greenhouse_location_options | raw }}
```

#### greenhouse_metadata_options

Returns a collection of `<option>` elements that represent a distinct list of the different metadata fields associated to greenhouse postings within site. This filter needs the results from an Orchard Core query ("AllGreenhousePostings" is created when enabling this module) that contains all the Greenhouse postings in order to access the metadata. If a `metadata` query string is present and has a matching value then the `<option>` element will have a `selected` attribute. This filter also requires the name of the property being accesed in the metadata.

Below is an example of how to use the filter.

```
{% assign allPostings = Queries.AllGreenhousePostings | query %}
{{ allPostings | greenhouse_metadata_options: property: "Project", selectedItem: project | raw }}
```

#### greenhouse_unique_departments

Returns an array of distinct departments. This filter needs the results from an Orchard Core query ("AllGreenhousePostings" is created when enabling this module) that contains all the Greenhouse postings in order to access the departments.

```
{% assign allPostings = Queries.AllGreenhousePostings | query %}
{% departments | greenhouse_unique_departments %}
```

#### greenhouse_unique_locations

Returns an array of distinct locations. This filter needs the results from an Orchard Core query ("AllGreenhousePostings" is created when enabling this module) that contains all the Greenhouse postings in order to access the locations.

```
{% assign allPostings = Queries.AllGreenhousePostings | query %}
{% locations | greenhouse_unique_locations %}
```

#### greenhouse_display_meta

Returns the value of a meta data field. This filter requires a content item with a `GreenhousePostingsPart` and a property name of a metadata field. 

```
{% assign project = Model.ContentItem | greenhouse_display_meta: property: "Project" %}
```

### Tracking

This module provides JS components that will interrogate HTML derived from the templates to send tracking events to Google Analytics. Events sent to Google Analytics makes use of the [ecommerce reporting](https://developers.google.com/analytics/devguides/collection/ga4/ecommerce?client_type=gtag) which will track when users are shown, click, view, begin applying and successfully complete an application.

## Packaging

When the theme is compiled (using `dotnet build`) it's configured to generate a `.nupkg` file (this can be found in `\bin\Debug\` or `\bin\Release`).

## Notes

This module was created using `v0.4.2` of [Etch.OrchardCore.ModuleBoilerplate](https://github.com/EtchUK/Etch.OrchardCore.ModuleBoilerplate) template.
