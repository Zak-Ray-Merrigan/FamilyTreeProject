# Family Tree Project
# Background
Initially, the family reunion committee is implementing a family tree in a word document and continually saving each update as PDF. After attending the reunion and looking at the physical copy of the tree, I thought to myself that I can automate this process by the applying the fundamemental concepts of Full-Stack Software programming. As a family reunion committee member, I am automating the family tree by writting a ASP.NET Core Full-Stack application that represents each family instance as a tree node in terms of Graph Theory as a MongoDB Collection record and in the DAO, Reading/Writing from a MongoDB Collection partitioned by family name as tree structure represented in Graph Theory.

# Resources
"2023 Pfingsten Book Alternate.docx": this is a word document that I am updating to save as a PDF.
"2023PfingstenBookAlternate.pdf": this is the file that is being uploaded for testing.
"appsettings.json": this is main settings of the project.
"firstEntrySample.json": shows a sample MongoDB Collection record

# FamilyTreeLibrary
This directory shows the server-side implementation of the project consisting the DAO and Service stages.
# FamilyTreeLibrary/Data
Represents the DAO Layer

# FamilyTreeLibrary/Models
Stores the logical/physical objects as model classes.

# FamilyTreeLibrary/Service
Represents the Service Layer

# FamilyTreeLibraryTest
This is a test project for the DAO and Service Layer of the server-side

# FamilyTreeSratch
This serves as scratch paper to be able to see stuff show up on the console.

# FamilyTreeAPI
Represent the Controller layer that can be tested using software such as postman.

# Getting Started with Create React App

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

The page will reload if you make edits.\
You will also see any lint errors in the console.

### `npm test`

Launches the test runner in the interactive watch mode.\
See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### `npm run build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.\
Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### `npm run eject`

**Note: this is a one-way operation. Once you `eject`, you can’t go back!**

If you aren’t satisfied with the build tool and configuration choices, you can `eject` at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (webpack, Babel, ESLint, etc) right into your project so you have full control over them. All of the commands except `eject` will still work, but they will point to the copied scripts so you can tweak them. At this point you’re on your own.

You don’t have to ever use `eject`. The curated feature set is suitable for small and middle deployments, and you shouldn’t feel obligated to use this feature. However we understand that this tool wouldn’t be useful if you couldn’t customize it when you are ready for it.

## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).