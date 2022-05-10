# Introduction

**Hive is an enterprise social network mobile application**, where coworkers or simply collaborators can **share the content** of different types to get **answers from other collaborators.** The app is also a step-by-step tutorial for the VScode extension called **Mobioos Forge** available on the [VScode marketplace](https://marketplace.visualstudio.com/items?itemName=Mobioos.mobioos-forge).

**Hive** is a functional application implemented with [Ionic](https://ionicframework.com/) (on [Angular](https://angular.io/)) and [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-5.0). It is built to illustrate the use of **Mobioos Forge** to create a Software Product Line (SPL) in order to manage software variability and create many variants for the same application.

✅ This project is the work result from [Mobioos.ai](http://www.mobioos.ai/). You will find all the explanations of the **Mobioos Forge technology** on the [dedicated website](https://forge.mobioos.ai) and its [documentation](https://documentation.mobioos.ai) 📓 <br />
🔗 You don't have the Mobioos Forge extension yet? [Download it from the VSCode Marketplace here](https://marketplace.visualstudio.com/items?itemName=Mobioos.mobioos-forge) <br />
👾 Do you have a question or something to say? Check out our [Discord community](https://discord.gg/t3ENBP3apg)

So, to return to the functionalities of the Hive application: The app offers the ability to create different groups and target them when creating "posts" so that they can be shared only with specific users. It also allows collaborators to create a group for a particular project and a limited period, share ideas with people related to that project, or create a group of friends with whom they want to share content privately.

This first [section](#user-guide) details the standard functionalities of the application. We will next talk about Hive's [Feature Model](#feature-model), its [Markers creation](#markers-creation), and its [customization process](#customization).

Regarding running the application, As Hive implements some conflicting features (for example **Fr** and **En** languages), it cannot run as it is. Running the app needs to generate a variant of the application.

# Branch presentations
This repository is organized with the following branches: 
  - `master`: The main branch that contains the SPL built with **Mobioos-Forge**.
  - `initial-application`: The initial application without any forge files.
  - `variants/forge-variants`: The variants generated [here](#customization).

# Pre-requisites
As Hive is written in Angular and .NET, you need to install: [.NET 5.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/5.0), [Node.js](https://nodejs.org/en/), and [npm](https://www.npmjs.com/) in order to run variants of the application.

Regarding Mobioos Forge, you can install the [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) extension in order to help **Mobioos Forge** find Maps while the Feature Mapping process.

# User Guide <span id="user-guide"/>

## Connection

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/connection/login.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/connection/register.png)

To access the application, users can create an account by clicking on the **"Register"** link and filling in the mandatory information, including the mail and password can then be used later on to connect. **Passwords must be at least 8 characters long and contain both a letter and a number**.

## The Flow Section

Once users are connected to the application, they have access to the first page of the application, the Flow. This is the central point of the application, where are regrouped all of the collaborators' shared content, presented as cards of different types depending on the content type., An "Idea" card, for example, can be answered by "Yes" or "No" and a "Report" card is automatically generated the day after the concerned card's expiration date to show its results.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/flow/flow.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/flow/report.png)


Users can navigate through the list of cards by swiping them to the left or right or decide to "Discard" one by swiping it to the top of the screen.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/flow/flow-navigation.png)


## The Highlights Section

The Highlights section of the application is shared by all the collaborators and shows much information.

The best contributor is the collaborator that has created the most posts in the past 30 days and has the most average answer number on these posts.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/highlight/best-contributor.png)

The best post shows the results of a post that expired in the past ten days that have the best (answers /views) score.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/highlight/best-post.png)

Finally, the top posts are non-expired posts (up to 5) with the best (answers /views) scores. It is possible to click on one of the list's items to show the card and answer it if it wasn't done before. Otherwise, it shows its results.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/highlight/top-post1.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/highlight/top-post2.png)


## The Posts Section

The post section is where users find their previously created posts, see their details and results, and create new ones. It shows a list of top posts that are the current user's posts with the most answers and the list of all the user's posts.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/posts/posts.png)

The post creation process starts when the user clicks on the create button in the top right corner and then needs to choose which type of post to create.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/posts/new-post.png)

Once the post type is selected, the user is invited to enter its content and choose a picture to display on the card or choose to use the default image. If the card is a question, it will then ask the user to create up to 5 answers available for the question. The card then needs to be configured. It requires a publication date that is the date at which the card will be available for other collaborators and an expiration date that is the date until which it will be available.

It is possible to make the card public available for every other contributor or target one or more specific groups to target particular collaborators.

Once all the card's settings have been correctly configured, a preview of the card displayed in the Flow is shown, and the user can choose to save it or go back and change the information. Once saved, the card will be available in the user's posts list and its results. It is essential to know that only the yet-to-be-published cards can be edited after being saved. Once a card is published, its content is definitive (however, it is still possible to delete it).

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/posts/new-idea.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/posts/post-settings.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/posts/idea-preview.png)

Clicking on one of the main page items will open the concerned post's details page, showing its different information chosen during its creation and others such as its status (published, not published, or closed) and its actual results.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/posts/idea-published.png)

It is possible from this page or the previous one to click on the menu icon (top right corner in the details page and on the end of the line of each item on the main page) to open a secondary menu giving access to the deletion of the post or its edition if it is in the "unpublished" status. When accessing the post edition, it is then possible to change all the card information chosen during its creation and save the changes.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/posts/edit-idea.png)

## The Account Section

The account section allows users to review and edit their profile information, manage their groups, change the application settings or sign out from the application.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/account.png)

## The Profile section

Users can view and edit any of their profile information from this page.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/profile.png)

## The Groups Management section

In this part of the application, users can see the group they are a member of. They cal also manage the ones they created and create new ones. This group system makes users able to target a specific group and, by extension, particular collaborators when creating cards to restrict persons' ability to answer these cards.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/groups/groups.png)

To create a group, users only need to enter a name, a country, and a city for the group before saving it.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/groups/create-group.png)

Once the group is created, it is directly possible to add any collaborators as members of this group and create cards targeting this group. The group creator can, of course, remove members from his groups as well.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/groups/group-created.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/groups/add-members.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/groups/group-finished.png)

The group's creator can remove members by clicking on their corresponding menu icon (at the end of each line of the list) and selecting the remove option or by accessing the same page used to add members and uncheck the corresponding checkboxes before saving. The members themselves can leave the group by clicking on their corresponding menu icon in the list of members and choosing "leave." This is also possible from the list of groups by clicking on the menu icon next to a group and choosing "leave" for groups of which the user is not the creator.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/groups/remove-user.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/groups/group-options.png)

Group creators can use the same icon in the group's list page to delete a group or edit it, cards targeting deleted groups will not be available anymore, but their results will be viewable by their creator. Editing a group is just a matter of changing its name, country, and city and has no further impact.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/groups/group-actions.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/user-guide/groups/edit-group.png)

## The Settings section

The settings page enables users to change the application language. English and French are available.

## The Sign Out section

This takes the user back to the login page to reconnect.

# SPL Overview <span id="spl-overview"/>

As presented in the [documentation](https://documentation.mobioos.ai) of Mobioos Forge, the first step to build a Software Product Line (SPL) consists of specifying the software variability by the usage of a **Feature Model**.

## Feature Model <span id="feature-model">

In the context of Hive, we identified several **Functionality Features** and **Resource Features** for this application. The following figure shows the Feature Model related to the *Hive application.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/feature-model/feature-model.png)

We will detail the different Functionality Features and Resource Features in the following sections.

### Functionality Features

#### The _Posts_creation_ Functionality Feature

This Functionality Feature represents the different types of posts that users can create. The Functionality Features **_Idea_**, **_Question_**, and **_Event_** are **mandatory**, meaning that they will always be enabled. Whereas the Functionality Features **_Quote_**, and **_Suggestion_** are **optional**. Thus, we can select one of the two, both, or none of them while the customization process.

#### The _Flow_display_ Functionality Feature

This Functionality Feature concerns the configuration of the flow display. The flow can be displayed as a stack of cards (as presented in the [user guide](#user-guide)) or as a scrollable list. We represented these two different displays as two sub-features (**_Cards_** and **_List_**) linked by a **Only one (Xor)** relation as they are mutually exclusive.

#### The _Like_ Functionality Feature

The **_Like_** Functionality Feature does pretty much what it says. It enables users to like cards in the flow.

We decided to make this Functionality Feature available only in the cards display mode. We chose to represent that relation as a **cross-tree constraint** (**_Like_ ⇒ _Cards_**) instead of making it a sub-feature of the ***Cards*** Functionality Feature. As it is optional, ***Like***  can be enabled/disabled during the customization process.

#### The _Default_language_ Functionality Feature

This Functionality Feature represents the application's default language (understand the language that the app will launch in).

It sets an **Only one (Xor)** relation between **French** and **English** because they are mutually exclusive.

### Resource Features

As proposed by **Mobioos Forge**, in addition to the Functionality Features, the Feature Model can also include **_Resource Features_**. While Functionality Features refer to the functionalities, Resource Features are related to the visual and data information of the application. This can include, for example, images, icons, and files.

#### The _Icon_ and _Splash_ Resource Feature

These Resource Features concern the icon and the splash screen image of the application (the image shown while the app is loading on launch).

#### The _Title_ Resource Feature

This Resource Feature is related to the name of the application. This name will be displayed on the device’s app.

#### The _Primary_color_ and _Secondary_color_ Resource Feature

These Resource Features are related to the primary and secondary colors used throughout the application’s design. They enable users to change the app’s identity according to their graphical chart.

If we do not choose these two Resource Features during the customization process, the variant application will have its original colors.

#### The Post images and colors Resource Feature

Each of the different post types (**_Idea_**, **Question\_**, **_Event_**, **_Quote_**, and **_Suggestion_**) has a Resource Feature child that represents its default picture (exception of **_Quote_** that has a default color). This design enables users to change the default look and feel for each type of content.

If these Resource Features are not filled, the app will use the default images (the default color for **_Quote_**).

If **_Quote_** and/or **_Suggestion_** are not selected, filling in the Resource Feature information is unnecessary, and the default image and color will be deleted from the variant.

However, If **_Quote_** and/or **_Suggestion_** are selected, we can choose whether or not to change the default color/picture as both Resource Features are optional.

## Markers Creation <span id="markers-creation"/>

The first step to map the Features to the code source is to create what **Mobioos Forge** called **_Markers_**.A _Marker_ represents an initial knowledge of the Feature in the source code that the **developer provides**.

Before starting to place Markers we need to analyze the code to have a better idea of what impact each Feature should have on the code.

### Analyzing the code base

Hive is separated as a frontend in _Ionic_ (4) and a backend in _ASP.NET Core_, linked to an _sqlserver_ database, which means we will have to place Markers in both the frontend and the backend to have consistent code and data after the customization process.

The application has been implemented following the 150% application concept. This means that the application contains the source code for every Feature in parallel, even the mutually exclusive ones. By consequence:
  1. The app in itself won’t build and run.
  2. It eases the mapping of Features as we mostly have to delete pieces of code related to disabled Features.

We could get the same result by not including the 150% of code and replacing existing code during the mapping. It depends on your preferences.

Now that we have covered the basics, let’s  map our Functionality and Resource Features into the source code. In this example, we will go over two Functionality Features and two Functionality Resources.

### _Suggestion_ Functionality Feature

The **_Suggestion_** Functionality Feature enables users to create suggestion posts. There is code for this Functionality Feature in both the frontend and the backend of the application. Thus, we have to place two sets of Markers.

#### Frontend

First, we know from the previous sections that the frontend codes for different types of posts are grouped in modules. Thus, we can add a *File-Marker* on the root directory of the suggestion module: `smartapp\frontend\src\app\modules\posts\modules\suggestion`.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/suggestion-folder-file-marker.png)

Here we want to analyze the files' content so we **answer yes when asked about the content analysis**.

This should take care of the _suggestion_ module. By creating a File-Marker on the directory, all sub-files are automatically set to be deleted when the Feature is not selected.

We can do the same thing to map the card component for suggestion posts:

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/suggestion-card-folder-file-marker.png)

With only these two File-Markers, we should be done with most of the suggestion mapping. Next, we want to make sure that no references are left. **Most of the references should get picked up by Mobioos Forge**. Still, there are some cases that we have to handle ourselves, like references in HTML files or lazy-loaded references.

For example, on the main posts page, we display a filtering menu to see only posts of specific types. It does not make sense to show the suggestion filter when the Feature is deactivated, so we need to map it with ***Suggestion***. So we add a *Code-Marker* inside the file `post-main.component.html`

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/post-main-code-marker.png)

Next, in files `choose-new-post-category.component.html` and `choose-new-post-category.component.ts`, we find a menu item that creates a new post as well as its corresponding behavior in the ts file. We add a Code-Marker on both of them.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/choose-new-post-html-code-marker.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/choose-new-post-ts-code-marker.png)

Note that in this case, we could only map in the HTML to hide the display and leave the typescript code as it is since it should never fall in this switch case. Nevertheless, we chose to map both anyway for code cleanliness' sake.

Next, we find two occurrences in the `flow.component.html` file that we need to map as well.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/flow-component-html-code-markers.png)

Last, we map the _Suggestion_ type in the `CardTypes` enum.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/frontend-cardtype-code-marker.png)

With all that, we should be done with the Markers creation step for the **_Suggestion_** Functionality Feature. Every reference that we left aside should get picked up by Mobioos Forge. We thus have the opportunity to map them later on.

To sum up, we:

1. Have mapped directories that contain suggestion-specific components and modules using File-Markers.
2. Have mapped lazy-loaded code that could not be detected by Mobioos Forge here with Code-Markers.
3. Have created Code-Markers on some HTML files to map pieces of display and related components if necessary.
4. Trust Mobioos Forge to pick up any leftover references in the code.

This is the most straightforward process to follow when the code is modular enough that you can delete big chunks of code. We will see in the following example that we sometimes need to be a bit more patient to make sure not to miss anything.

The next section describes the Markers related to the backend of the ***Suggestion*** Functionality Feature.

#### Backend

In the backend source code, we can find the `Suggestion` class that is used for each type of post. Here we decide to add a File-Marker on the file `Suggestion.cs`. Thus, if the **_Suggestion_** Functionality Feature is disabled, the file will be deleted.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/suggestion-cs-file-marker.png)

Now, we will look for other references. We have a `Suggestion` type in the `CardTypes` enumeration. We can add a Code-Marker on it.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/backend-cardtype-code-marker.png)

Next, in the file `DbInitializer.cs`, we have a method that creates the default picture for the suggestion card. We can add a Code-Marker to this method.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/db-initializer-code-marker.png)

Last, the `Program.cs` file contains a property called `suggestionDefaultPic` that gets the `PictureId` of the _Suggestion_ card. We add a Code-Marker for this variable.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/program-cs-code-marker.png)

The remaining references should get picked up by Mobioos Forge.

To sum up, we:

- Have created a File-Marker on the `Suggestion.cs` file.
- Have created Code-Markers on variables and methods used by the **_Suggestion_** Functionality Feature.
- Trust Mobioos Forge to retrieve the remaining references in the code and create Maps from it.

Now that we added the Markers, we can start validating the mappings computed by Mobioos Forge showed in the [Feature-Maps view](https://documentation.mobioos.ai/?id=feature-mapping-using-the-feature-maps-view). Most maps validation are straightforward as the best way to map them is to map the overall class, method or statement.

### _Flow_display_ Functionality Feature

We will now take care of the Feature **_Flow_display_** and its children: **_Cards_** and **_List_**. The following figure illustrates the **_Flow_display_** Functionality Feature from the initial [Feature Model](#feature-model) for a quick reminder.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/feature-model/zoom-flow-display.png)

We can see that the Feature's children are mutually exclusive since they are linked through an **Only one (Xor)** relation. We remember that the app has been implemented following the 150% app approach, meaning that each Feature's code is present at the same time. The idea here will be thus to map these implementations to the corresponding Feature to delete one or the other during the customization process.

Unlike the **_Suggestion_** Feature that we saw earlier, the **_Cards_** and **_List_** Features are not separated in their modules. So, we will not be able to delete big chunks of code here.

Even though it is mostly a display issue, we will also place Markers in the _typescript_ files to further clean the code from unused code.

First, we add Code-Markers on the HTML file for the flow. We can see two `ng-containers` containing a list each. **The first one displays the cards stack**, and **the second one shows the list**. We place our Code-Markers accordingly as shown in the figure below.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/flow_display/html-code-markers.png)

The first Code-Marker is linked to the ***Cards*** Functionality Feature, whereas the second Code-Marker is linked to the ***List*** Functionality Feature.

Now, let’s clean up the _typescript_ file. We mostly have to place Code-Markers for the **_Cards_** Functionality Feature as it is the one needing the most extra methods to handle the cards stack, whereas the **_List_** display only has one specific method.

First, we add a Code-Marker on several properties of the class specific to the **_Cards_** Functionality Feature:

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/flow_display/ts-code-markers.png)

These are used to handle the stack of cards and can be deleted when the **_List_** Feature is selected. Once the Code-Marker is added, Mobioos Forge will look for occurrences and create Maps from them.

Regarding the **_List_** display, we only need to mark the `onCardListAnswered` method.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/flow_display/ts-list-code-markers.png)

Now that we are done with the flow components, we have to take care of the card components; quick reminder, each type of post has its display component in the shared module. These display components contain the code for each type of display. We do the same work for every one of these components. Let’s take the event card just like in the flow. In the file `event-card.component.html`, we have two `div`, each containing one display, the first for the cards display and the second for the list display. We add the corresponding Code-Markers on these.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/flow_display/event-card-html-card-code-markers.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/flow_display/event-card-html-list-code-markers.png)

Next, we only have a Code-Marker to add in the related typescript file (`event-card.component.ts`). This Code-Marker is added on the property `activeCardId` that is used to animate the card when it comes on top of the stack:

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/flow_display/event-card-ts-card-code-markers.png)

We then repeat these operations on every card component and should be done with the Markers creation of these Features.

Next, you can follow the same process for every Feature. Note that you are not limited in terms of Markers. You can thus add Markers everywhere you see fit.

### _Primary_color_ Resource Feature

The primary color of the application is the color used throughout the app to give it a coherent design. In this app, it is used in some important text and mainly as a general background color. By default, it is a nice dark purple (`#4324B0`), and since the colors are optional Resources, if the user chooses not to customize it will use this color. This Resource impacts only the frontend of the application in our case.

To map this color (and the secondary one alike), we need to find its declaration. In our case, it is located in the `smartapp/frontend/src/theme/variables.scss` file. We add a Code-Marker on the declaration value.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/primary_secondary_colors/variables-scss-code-markers.png)

Since the Resource Feature is a Color, its configuration during the customization process will be presented with a color picker. Thus, the chosen hexadecimal value will replace the mapped one.

### _SuggestionImage_ Resource Feature

The **_SuggestionImage_** Resource Feature represents the default picture for the suggestion card.

To add a Code-Marker to this Resource Feature, we have to find the URL referencing this picture. On the method `CreateSuggestionDefaultPic` of the _DbInitializer.cs_ file. We then add a Code-Marker on the given URL.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/markers/suggestion/suggestion-image-db-initializer-code-marker.png)

This concludes the creation of Markers for the Hive application. You can now follow the same processes to add Markers for the remaining Features. You can now start validating them as well as their related Maps. Most maps validation are straightforward as the best way to map them is to map the overall class, method or statement.

## Customization <span id="customization"/>

Once all the Markers are created and their related Maps are validated, we can try to generate a variant of the Hive application. This enables us first to try out our Features relations and make sure that our Feature Model is correct. For example, if we select the **_List_** Functionality Feature, we see that **_Cards_** and **_Like_** Functionality Features are deactivated as those are mutually exclusive (**_List_** and **_Cards_** through their **Only One** relation, **_Like_** is linked to **_Cards_** via a constraint).

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/customization/zoom-flow-display-list-enabled.png)

Here we will generate a variant where the Functionality Feature ***Quote*** and ***Suggestion*** are disabled. Regarding the UI of the frontend, we choose a ***List*** display with the ***English*** language. We also provide several new images for the post creation. All used images are available in folder `.mobioos-forge/customization-file-resources` We named this configuration `no-quote-and-suggestion-list-en`.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/customization/configuration.png)

A new project is then created in the `$HOME_DIR/mobioos-forge-customization/` folder with the name we used when creating the configuration. In this folder, we find our customized code. We can, for example, check that the methods that we set as deleted earlier are effectively deleted.

We can also, of course, launch the app and try it out to see the difference and test everything out.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V3/spl/customization/forge-variant.png)

### Running the backend
On a terminal, go inside the folder `smartapp/backend/` and run the following command: `dotnet run`

### Running the frontend
On a terminal, go inside the folder `smartapp/frontend/` and run the following commands:
- `npm install`
- `npm start`

# Conclusion
In summary, to be able to derivate an application:

- Install the **Mobioos Forge** extension.
- Identify Functionality Features and Resource Features of your application using the extension’s Feature Model Designer.
- Set Markers for your Features.
- Validate the added Markers and computed Maps.
- Finally, derivate the application.

Using the **Mobioos Forge** extension is intuitive and straightforward. Just follow the steps described above, and you can derivate any application.
