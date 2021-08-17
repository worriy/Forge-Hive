# Introduction

Hive can be considered as an **enterprise social network mobile application**, where coworkers or simply collaborators can **share the content** of different types to get **answers from other collaborators.** The app is also a step-by-step tutorial of _**How to use Mobioos Forge**_, the newest plugin from **MOBIOOS** for the **#VSCode** **Marketplace**.

**HIVE** is a functional application to illustrate the use of **Mobioos Forge** to manage software variability and create by feature customization many variants for the same application. 


✅ This project is the work result from [Mobioos.ai](http://www.mobioos.ai/). You will find all the explanations of the **Mobioos Forge technology** on the [dedicated website](https://forge.mobioos.ai) and its [documentation](https://documentation.mobioos.ai) 📓 <br />
🔎 Further down in the documentation, you will find **How we implemented Mobioos Forge to help us create different variants for HIVE**. <br />
🔗 You don't have the Mobioos Forge extension yet? [Download it from the VSCode Marketplace here](https://marketplace.visualstudio.com/items?itemName=Mobioos.mobioos-forge) <br />
👾 Do you have a question or something to say? Check out our [Discord community](https://discord.gg/wWR3z5nc)


So, to return to the functionalities of the HIVE application: The app offers the ability to create different groups and target them when creating "posts" so that they can be shared only with specific users. It also allows collaborators to create a group for a particular project and a limited period, share ideas with people related to that project, or create a group of friends with whom they want to share content privately.

This first section details the standard functionalities of the application (its original specifications). Click [here](#variability-specification) to jump into the **variability specification** section or [here](#variability-implementation-marking) to learn more about the **variability implementation**.


# User Guide <span id="user-guide"/>

## Connection

<div class="row">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image001.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image002.png)

</div>

To access the application, users can create an account by clicking on the **"Register"** link and filling in the mandatory information, including the mail and password can then be used later on to connect.

## The Flow Section

Once users are connected to the application, they have access to the first page of the application, the Flow. This is the central point of the application, where are regrouped all of the collaborators share content, presented as cards of different types depending on the content type., An "Idea" card, for example, can be answered by "Yes" or "No" and a "Report" card is automatically generated the day after the concerned card's expiration date to show its results.

<div class="row">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image004.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image006.png)

</div>

Users can navigate through the list of cards by swiping them to the left or right or decide to "Discard" one by swiping it to the top of the screen. 

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image008.png)

</div>

## The Highlights Section

The Highlights section of the application is shared by all the collaborators and shows much information.

The best contributor is the collaborator that has created the most posts in the past 30 days and has the most average answer number on these posts.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image009.png)

</div>

The best post shows the results of a post that expired in the past ten days that have the best (answers /views) score.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image010.png)

</div>

Finally, the top posts are non-expired posts (up to 5) with the best (answers /views) scores. It is possible to click on one of the list's items to show the card and answer it if it wasn't done before. Otherwise, it shows its results.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image012.png)

</div>

<div class="row">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image014.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image016.png)

</div>

## The Posts Section

The post section is where users find their previously created posts, see their details and results, and create new ones. It shows a list of top posts that are the current user's posts with the most answers and the list of all the user's posts.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image018.png)

</div>

The post creation process starts when the user clicks on the create button in the top right corner and then needs to choose which type of post to create.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image019.png)

</div>

Once the post type is selected, the user is invited to enter its content and choose a picture to display on the card or choose to use the default image. If the card is a question, it will then ask the user to create up to 5 answers available for the question. The card then needs to be configured. It requires a publication date that is the date at which the card will be available for other collaborators and an expiration date that is the date until which it will be available.

It is possible to make the card public available for every other contributor or target one or more specific group(s) to target particular collaborators.

Once all the card's settings have been correctly configured, a preview of the card displayed in the Flow is shown, and the user can choose to save it or go back and change the information. Once saved, the card will be available in the user's posts list and its results. It is essential to know that only the yet-to-be-published cards can be edited after being saved. Once a card is published, its content is definitive (however, it's still possible to delete it).

<div class="row">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image021.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image023.png)

</div>

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image025.png)

</div>

Clicking on one of the main page items will open the concerned post's details page, showing its different information chosen during its creation and others such as its status (published, not published, or closed) and its actual results.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image027.png)

</div>

It is possible from this page or the previous one to click on the menu icon (top right corner in the details page and on the end of the line of each item in the main page) to open a secondary menu giving access to the deletion of the post or its edition if it is in the "unpublished" status. When accessing the post edition, it is then possible to change all the card information chosen during its creation and save the changes.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image029.png)

</div>

## The Account Section

The account section allows users to review and edit their profile information, manage their groups, change the application settings or sign out from the application.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image031.png)

</div>

## The Profile section

Users can view and edit any of their profile information from this page.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image033.png)

</div>

## The Groups Management section

In this part of the application, users can see and manage the groups they are a member of the ones they created and create new ones. This group system makes users able to target a specific group and, by extension, particular collaborators when creating cards to restrict persons' ability to answer these cards.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image035.png)

</div>

To create a group, users only need to enter a name, a country, and a city for the group before saving it.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image037.png)

</div>

Once the group is created, it is directly possible to add any collaborators as members of this group and create cards targetting this group. The group creator can, of course, remove members from his groups as well.

<div class="row">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image039.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image041.png)

</div>

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image043.png)

</div>

The group's creator can remove members by clicking on their corresponding menu icon (at the end of each line of the list) and select the remove option or by accessing the same page used to add members and uncheck the corresponding checkboxes before saving. The members themselves can leave the group by clicking on their corresponding menu icon in the list of members and choosing "leave." This is also possible from the list of groups by clicking on the menu icon next to a group and choosing "leave" for groups of which the user is not the creator.

<div class="row">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image045.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image047.png)

</div>

Group creators can use the same icon in the group's list page to delete a group or edit it, cards targetting deleted groups will not be available anymore, but their results will be viewable for their creator. Editing a group is just a matter of changing its name, country, and city and has no further impact.

<div class="row">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image049.png)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image051.png)

</div>

## The Settings section

The settings page enables users to change the application language. English and French are available.

## The Sign Out section

This takes the user back to the login page to reconnect.

#  Variability specification - The feature model <span id="variability-specification"/>

As presented in the [documentation](https://documentation.mobioos.ai) of Mobioos Forge, the first step consists of defining the application's functionalities and explicitly specifies software variability using  **feature models**.

In the context of **HIVE**, we identified several features and resources for this application. The following figure shows the feature  model related to the **HIVE** application.

We'll detail the different features and ressources in the following.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image053.png)

</div>

## Features

### The _Posts_creation_ feature

This feature node represents the different types of posts that users can create. The features **_Idea_**, **_Question_**, and **_Event_** are **mandatory**, meaning that they’ll always be selected (they are part of the default package of the feature of the app) while the features **_Quote_**, and **_Suggestion_** are **optional**. Thus, we can select one of the two, both, or none of them when derivating.

### The _Flow_display_ feature

This feature concerns the configuration of the flow display. The flow can be displayed as a stack of cards (as presented in the [user guide](#user-guide)) or as a scrollable list. We represented these two different displays via two child nodes (**_Cards_** and **_List_**) linked by a **XOR** relation as they are mutually exclusive.

### The _Like_ feature

The **_Like_** feature does pretty much what it says. It enables users to like cards in the flow. 

We decided to make this feature available only in the cards display mode, and we chose to represent that relation as a **cross-tree constraint** (**_Like_ ⇒ _Cards_**) instead of making it a child of the cards feature. This feature being optional, it can be selected/unselected during the derivation process.

### The _Default_language_ feature

This feature represents the application's default language (understand the language that the app will launch in).

It sets a **XOR** relation between **French** and **English** because they are mutually exclusive.

## Resources

As proposed by the **Mobioos Firge** platform, in addition to the feature nodes, the feature model can also include what is referred to as **_resource  nnodes_**. While feature nodes refer to functionalities, resources are related to the visual and data information of the application. This can include, for example, images, icons, and files.

As well as features we have some resources nodes for **HIVE**. Unlike features, these will have less impact on the code itself after derivation but will change some specific parts.

### The _Icon_ and _Splash_ resources

These resources concern the icon and the splash screen image of the application (the image shown while the app is loading on launch).

### The _Title_ resource

This resource is related to the name of the application. This name will be displayed on the device’s app.

### The _Server_URL_ resource

This resource enables the front-end application to know what URL to use when sending requests to the back-end. It will rewrite the default base URL of the app.

### The _Primary_color_ and _Secondary_color_ resources

These resources are related to the primary and secondary colors used throughout the application’s design. They enable users to change the app’s identity according to their graphical chart.

If we don’t choose these two resources (optional), the derivated application will have its original colors. 

### The Post images and colors resources

Each of the different post types (**_Idea_**, **Question_**, **_Event_**, **_Quote_**, and **_Suggestion_**) has a resource child that represents its default picture (exception of **_Quote_** that has a default color). This design enables users to change the default look and feel for each type of content. 

If these resources are not filled, the app will use the default images (the default color for **_Quote_**) (seen in the user guide).

If **_Quote_** and/or **_Suggestion_** isn’t selected, filling in the resource information is unnecessary, and the default image and color will be deleted from the derivated application. We chose to use constraints to define this relation (**_Quote_ ⇒ _QuoteColor_**) and (**_Suggestion_** => **_SuggestionImage_**).

However, If **_Quote_** and/or **_Suggestion_** are selected, we can choose whether or not to change the default color/picture as both resources are optional.

# Variability implementation - Marking <span id="variability-implementation-marking"/>
## The code base
The first step to map features to the code source is to create what is called by **Mobioos Forge** **_markers_**. A marker is a code fragment that is related to a specific feature or resource. It is provided by developers to shows initial information that will be  used during the feature mapping step.


Before starting to place markers we need to analyze the code to have a better idea of what impact each feature should have on our code.

Hive is separated as a front-end in _Ionic_ (4) and a back-end in _dotnet 5.0_, linked to an _sqlserver_ database, which means we will have to place markers in both the front-end and the back-end to have a consistent derivated code and consistent database data.

The application has been implemented following the 150% application concept. This means that the application contains the source code for every feature in parallel, even the mutually exclusive ones. By consequence:
 1. The app in itself won’t build and run.
 2. It will ease the mapping of features as we will mostly have to delete pieces of code related to disabled features.

We could get the same result by not including the 150% of code and replacing existing code during the mapping. It depends on your preferences.

## Placing markers

Creating markers is relatively easy with Forge. You select the code fragment you want to place a marker on: **Click on the yellow light bulb → Mobioos-Forge: add a functionality/resource marker**. Or select a folder or file, then **Right click and select Mobioos-Forge → File markers → Add a file marker.**

**Mobioos Forge** will then ask the feature/resource this marker is related to. We simply select the feature from the list. Keep in mind that to place markers for a feature/resource, you need to be in the correct **variability configuration**. The options presented in the above menu depend on this. To change the variability configuration, simply click on the button in the bottom bar to switch between resource or functionality variability.

Once a marker is applied, it will show with the node’s marker color (set in the feature model) as background if the parameter is activated in the extension’s settings. If not, it will only indicate the presence of a marker by displaying the marker color in the line start. Note that this will display only the markers according to the current configuration (functionality variability will show only features markers, and resource variability will show only resources markers).

## Front-end marking
Now that we’ve covered the basics, let’s place markers for our features and resources. In this example, we will go over two features and two resources.

### Features Marking

#### Marking the _Suggestion_ Feature
The **_Suggestion_** feature enables users to create suggestion posts. There is code for this feature in both the front-end and the back-end of the application, which means we will have to place two sets of markers.

First, we know from the previous sections that the front-end codes for different types of posts are grouped in modules. We can then take advantage of the _File Marker_ type and place a marker directly on the root directory of the suggestion module. **Right-click on the suggestion directory → Mobioos-Forge → File Markers → Add a file marker.**

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image083.gif)

</div>

Next, we select the **_Suggestion_** feature.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image085.png)

</div>

This should take care of the _suggestion_ module. By creating a marker on the directory, all sub-files are automatically set to be deleted when the feature is not selected.

We can do the same thing to mark the card component for suggestion posts:

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image087.png)

</div>

With only these two markers, we should be done with most of the suggestion marking. Next, we’ll want to make sure that no references are left. Most of the references should get picked up by the analyzer. Still, there are some cases that we have to handle ourselves, like references in HTML files or lazy-loaded references.

Let’s handle the lazy-loaded references first. It is pretty straightforward in our case as it only refers to the routing file of the post-module. We’ll mark the suggestion route as follows.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image151.gif)

</div>

We then need to find other and less evident “references”/feature related code fragments. For example, in the main posts page, we display a filtering menu to see only posts of specific types. It doesn’t make sense to show the suggestion filter when the feature is deactivated, so we’ll need to remove that from the HTML.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image152.gif)

</div>

Next, in the choose-new-post-category component, we find a menu item to access the creation and related component methods. We’ll mark them both.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image153.gif)

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image154.gif)

</div>

Note that in this case, we could only mark in the HTML to hide the display and leave the method as is since it should never fall in this switch case, we choose to mark both anyway for code cleanliness sake.

Next, we’ll find two occurrences in the _flow.html_ file that we need to mark as well.

<div class="align-center">


![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image155.gif)

</div>

Last, we mark the _Suggestion_ type in the `CardTypes` enum.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image156.gif)

</div>

With all that, we should be done with the marking of the **_Suggestion_** feature. Every reference that we left aside should get picked up by the analyzer, and we will have the opportunity to map them later on.

To sum up the marking of the **_Suggestion_** feature, we:
  1. Have marked directories that contain suggestion-specific components and modules.
  2. Have marked lazy-loaded code that cannot be detected by the code analyzer here.
  3. Have marked HTML files to remove pieces of display and related components if necessary.
  4. Trust the analyzer to pick up any leftover references in the code.

This is the most straightforward process to follow when the code is modular enough that you can delete big chunks of code. We’ll see in the following example that we sometimes need to be a bit more patient to make sure not to miss anything.

#### Marking the _Flow_display_ Feature

We will now take care of a somewhat more difficult feature, the **_Flow_display_**, and more precisely, its children, the **_Card_** display, and the **_List_** display. The following figure illustrates the **_Flow_display_** feature from the initial feature model for a quick reminder.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image103.png)

</div>

We can see that the children of the features are mutually exclusive since they are linked through a **Xor** relation. We remember that the app has been implemented following the 150% app approach, meaning that each feature's code is present at the same time. The idea here will be thus to map these implementations to the corresponding feature to delete one or the other during the derivation process.

We'll start as always by adding markers. Unlike the **_Suggestion_** feature that we saw earlier, the **_Cards_** and **_List_** features are not separated in their modules. So, we will not be able to delete big chunks of code here.

Even though it's mostly a display issue, we will also place markers in the _typescript_ (.ts) files to further clean the code from unused code.

First, we will mark the HTML file for the flow. We can see two ng-containers containing a list each. The first one displays the cards stack, and the second one shows the list. We'll place our markers accordingly.

The first marker for the **_Cards_** feature is illustrated as follows:

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image157.gif)

</div>

The second one for the **_List_** feature.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image158.gif)

</div>

Note that we could have placed the markers only on a piece of those code fragments. For example, the point here is that we need the mapper to allows us to decide what to do with this code fragment, but the marker doesn’t have to take in the whole code source for this. It will ease the mapping this way as we won’t have to think about what code fragment to delete. This code fragment will be selected according to the marker. It’s only a question of preferences. Markers placement in HTML files is not as important as in other files since the analyzer won’t pick up references anyway.

Now, let’s clean up the _typescript_ (.ts) file. We mostly have to place markers for the **_Cards_** feature as it’s the one needing the most extra methods to handle the cards stack, whereas the **_List_** display only has one specific method handling answers.

First, we mark the class properties specific to the **_Cards_** feature:

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image159.gif)

</div>

These are used to handle the stack of cards and can be deleted when the **_List_** feature is selected. This marker will enable the analyzer to search for references of those properties and let us map the necessary actions.

Next, we will place a marker on each method specific to the cards display. There are several ones:
`onNextCard`, `onPreviousCard`, `onAdaptCardsStack`, `onDiscard`, `onSwipeCard` and `onCardAnswered`.

We place the markers on each method declaration like this:

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image160.gif)

</div>

This way, the analyzer will find other references to those methods, and we will delete the calls when needed.

Regarding the **_List_** display, we only need to mark the `onCardListAnswered` method.

Now that we’re done with the flow components, we have to take care of the card components; quick reminder, each type of post has its display component in the shared module. These display components contain the code for each type of display. We’ll do the same work for every one of these components. Let’s take the event card just like in the flow. We have two divs, each containing one display, the first for the cards display and the second for the list display. We add the corresponding markers on these.


<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image161.gif)

</div>



Next, we only have a marker to add in the .ts file for the cards display on this property that is used to animate the card when it comes on top of the stack:

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image162.gif)

</div>


We then repeat these operations on every card component and should be done with marking these features.

We will follow the same process for every feature to have markers for each. Note that you are not limited in terms of markers. You can thus mark every occurrence that you see if it’s more comfortable but trusting the analyzer to detect references and using it at our advantage enables us to be more efficient.

### Resources Marking

To mark resources, we need to make sure to switch mode to the resource variability by clicking on the corresponding button in the bottom bar:

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image119.png)

</div>

Once we click this button, it will switch to resource variability:

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image121.png)

</div>

To illustrate resources marking, we’ll consider the  **_Primary_color_** resource.

#### Marking the _Primary_color_ Resource

The primary color of the application is the color used throughout the app to give it a coherent design. In this app, it is used in some important text and mainly as a general background color. By default, it’s a nice dark purple (`#4324B0`), and since the colors are optional resources, if the user chooses not to customize it will use this color. This resource impacts only the front-end of the application in our case.

To mark this color (and the secondary one alike), we need to find its declaration. In our case, it’s located in the _theme/variables.scss_ file and put a marker on the color value.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image166.gif)

</div>
Since the resource is marked as a color resource, its configuration during the derivation process will be presented with a color picker. Thus, the chosen hexadecimal value will replace the marked one. 


## Backend Marking

### Features Marking

#### Marking the **_Suggestion_** Feature

In the back-end source code, we have a class for each type of post to use the file marking feature and place a marker on the _Suggestion_ file.  If the **_Suggestion_** feature is not activated, the file will be deleted.

To add this file marker, we go to the _Suggestion_ file:  **Right-click on the file and select  → Mobioos-Forge → File Markers → Add a file marker.**

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image125.png)

</div>

Then we choose what feature this marker is related to. We select the **_Suggestion_** feature.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image127.png)

</div>

Once the marker is applied, you will notice that the file's name has changed color, with the "MF" displayed on the right.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image129.png)

</div>

Now, we will look for other references. We have a `Suggestion` type in the `CardTypes` enumeration. We select the string `Suggestion`: **Click on the yellow light bulb → Mobioos-Forge: add a functionality marker.**

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image147.gif)

</div>

Once the marker is applied, it will show with the node’s marker color as the background.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image133.png)

</div>

Next, in the file _DbInitializer.cs_, we have a method that creates the default picture for the suggestion card. We will add a marker to this method.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image148.gif)

</div>

Last, we have a global variable to get the _PictureId_ of the _Suggestion_ card. In the _program.cs_ file, we will add a marker for this variable.  

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image149.gif)

</div>

The remaining references should get picked up by the analyzer.

To sum up, we:

- Have marked the _suggestion_ file.
- Have marked variables and methods used by the **_Suggestion_** feature.
- Trust the analyzer to retrieve the remaining references in the code.

### Resources Marking

Before starting to add markers for resources, we need to be in the correct setup. We click on the button in the bottom bar to switch to **Resource variability**.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image141.png)

</div>

#### Marking the _SuggestionImage_ Resource

<div class="align-center">

The **_SuggestionImage_** resource represents the default picture for the suggestion card.

</div>

To add a marker to this resource, we have to find the URL referencing this picture. On the method `CreateSuggestionDefaultPic` of the _DbInitializer.cs_ file: **Select the URL → Click on the yellow light bulb → Mobioos-Forge: add a resource marker.** Then select the **_SuggestionImage_** feature.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image165.gif)

</div>

The marker is added.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image144.png)

</div>

# Variability implementation - Mapping and derivation

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image054.png)

</div>

Once the markers have all been placed ([see the Marking section](#variability-implementation-marking)), we can start the **feature mapping process**. This process will take our markers as input and map them into the application's source code. The feature mapping process will enable us to specify how to act on the code when features are unselected.

For example, suppose we placed a marker on a specific component field during the marking process. In that case, the analyzer will ask us how to act on every field reference, enabling us to delete or replace bits and pieces of code for the actual feature. 

If we place a marker on some files, they will be automatically deleted when generating a variant in which the feature is not selected.

After placing our markers, we will have to launch the mapping process and set actions to take on the related pieces of code.

Note that mapping a feature means determining what action to take when the feature is NOT selected. Mapping a resource is quite different as the resource's marker will be replaced with the selected value during the derivation process.

## The feature Mapping Process

During the mapping process, the extension will take us from marker to marker (and to references of those markers) to decide how to act on the code when the currently mapped feature is not selected. 

The extension offers two options:

- Map all features: the mapping will be done feature after feature and will end with the mapping of resources (that will not precisely call for actions on our side).
- Map selected features/resources: this will come in handy when we have done a complete run of mapping and need to re-map one or several specific features. After selecting this option, the extension will ask us what features we want to map.

To launch the mapping, we have to **Right-click on the feature model file and select Mobioos-Forge → Feature Mapping → Map all features (or the other options).**

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image055.gif)

</div>

Once the mapping is launched, Mobioos  Forge will open the file where the first marker is found. The feature being mapped is shown at the bottom.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image057.png)

</div>

We will have the possibility to select the code fragment to change and select its related action using **CTRL + Q**. By doing so, we can then select the related action to perform. Possible options are: _delete_, _replace_, _ignore_ and _cancel_.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image059.png)

</div>

- Selecting _delete_ will set this part of the code to be deleted when the current feature isn’t selected.
- Selecting _replace_ will show another menu that enables us to select an already existing replacement or to create a new one. This replacement will then be applied when the related feature is not selected. if we choose to create a new replacement, a new file will open beside the current file and enable us to write the code we want to replace the selected one. <br/>Once the replacement code is written, you can save it. The extension will then ask for the name of the newly created replacement (the name is pre-filled with a suffix to show what feature it’s related to). The three figures below show in detail those steps.
- Selecting _ignore_ will ignore that marker and not set any mapping for it. It has the same effect as clicking on nothing (deselecting the marker) and using **CTRL + Q**.
- Selecting _cancel_ will completely end the mapping process.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image061.png)
<p style="text-align: center;">Selected an already existing replacement.</p>

</div>

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image063.png)
<p style="text-align: center;">Writing the content of the replacement.</p>

</div>

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image065.png)
<p style="text-align: center;">Writing a name for the new replacement.</p>

</div>

In our case, and as stated earlier, we will mainly use the _delete_ option to remove pieces of code.

We fill the name as wanted and press enter to validate the selection and finish the replacement creation. This replacement will be used when the feature isn’t selected.



### Mapping the features

We will continue the mapping with the two features that we have marked: **_Suggestion_** and **_Flow_display_**

#### The **_Suggestion_** feature

This feature has markers in the front-end and in the back-end. The mapper will mix between the back-end and front-end markers.

- The first marker is in the back-end. It is a string “Suggestion” in a method that retrieves the default image of a card type.
- We select the piece of code in the Suggestion section, then **CTRL + Q**, and choose the deletion action.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image066.gif)

</div>

- Then, we pass to the method `CreateSuggestionDefaultPic`, which creates the default image of the suggestion card in the database. This method is used when launching the back-end for the first time.
- We select the whole method, then **CTRL + Q**, and choose the _delete_ action.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image068.png)

</div>


- Now, we pass to the front-end, we have a method that contains a piece of code related to the suggestion feature. So we will do the same thing as we did previously. Select the code fragment then **CTRL + Q** and we select _delete_.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image069.gif)

</div>

At some point, we’ll reach our HTML file(s).

- We will do the same thing, we select the parts related to the **_Suggestion_** feature and delete it.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image070.gif)

</div>


Regarding the file markers, the related files will be automatically deleted when generating a variant where the corresponding features are unselected. Thus we also need to find references to these files. Forge will discover the paths of these files and ask us what we want to do for each reference. As previously, we can choose to _delete_, _replace_ or _ignore_ these paths.

At the end of the feature mapping process, Forge will search for references to files marked with a file marker. In our case, these paths are declared inside the files, which are also marked with the **_Suggestion_** feature, so we will ignore them.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image071.gif)

</div>

#### The _Flow_display_ feature

Now, we will map the children of the feature **_Flow_display_**, the **_Cards_** display, and the **_List_** display.

- The first marker of the Cards feature is on this method used in the Flow component to trigger when a card is answered.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image073.png)

</div>


- We select the whole method, then **CTRL + Q**, and we choose the Deletion action.
- After that, the mapping process will continue to the next marker or reference to that method. In our case, it will find all the markers for the **_Cards_** feature and the references of the removed methods. 

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image074.gif)

</div>


At some point, we’ll reach our HTML file(s), 

- The mapping works the same in those files. We can _delete_ the parts related to the **_Cards_** feature
- We placed our markers on this whole piece of code to avoid thinking about it during the mapping.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image075.gif)

</div>

After finishing the mapping of the **_Cards_** feature, we continue now with the List feature. We have to follow the same process. <br/>
In the **_List_** feature, only one method is called in the HTML file. Thus the mapping will be faster than the previous one.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image076.gif)

</div>

### Mapping the resources


The resource mapping is more straightforward than the feature mapping, which will not exactly call for actions on our side.

#### The _SuggestionImage_ and _Primary_color_ resources

When we are on the **_SuggestionImage_** resource, the mapper asks us if we want to look for references to this file or not. We put “Yes” in our case so that the mapper finds other references to this file. Regarding the **_Primary_color_**, the mapper will not ask us anything.

## Derivation

To start the derivation process and customize our application, we can  **Right-click on the feature model file and select → Mobioos-Forge → Create Configuration**. Mobioos Forge will then ask us to name that configuration. We can select any name and use Enter to open 
<div class="align-center">the configuration editor.

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image078.gif)

</div>

This enables us first to try out our features relations and make sure that our feature model is correct. For example, if we select the **_List_** feature, we see that **_Cards_** and **_Like_** features are deactivated as those are mutually exclusive (**_List_** and **_Cards_** via their **XOR** relation, **_Like_** is linked to **_Cards_** via a constraint)

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image080.png)

</div>

We can now try out our variability by filling the necessary fields and selecting the needed features. Click on save, which will trigger showing errors if there are some. We then have to click on derivate and wait for the magic to happen.

<div class="align-center">

![alt text](https://mobioosstorageaccount.blob.core.windows.net/public-documentation/Hive-Doc-V2/image082.png)

</div>

A new project will be created in the **HOME_DIR/mobioos** folder with the name we used when creating the configuration, in which we’ll find our derivated code. We can, for example, check that the methods that we set as deleted earlier are effectively deleted.

We can also, of course, launch the app and try it out to see the difference and test everything out.

In summary, to be able to derivate an application: 
- Install the **Mobioos Forge** extension.
- Identify features and resources of our application using the extension’s feature model.
- Set markers for our features and resources created in the feature model.
- Map the markers we added.
- Finally, derivate our application.

Using the **Mobioos Forge** extension is intuitive and straightforward. Just follow the steps described above, and you can derivate any application.
