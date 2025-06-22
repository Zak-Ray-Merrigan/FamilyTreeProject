/**
 * Enums.ts
 * 
 * This file contains various enumerations used throughout the family tree application.
 * These enums help to maintain consistency and type safety across the codebase.
 */

/**
 * Represents the type of person in the family tree.
 */
export enum PersonType {
    Member = 'Member',  // A direct member of the family
    InLaw = 'InLaw'     // Someone who married into the family
};

/**
 * Represents different loading contexts or operations in the application.
 * This enum is likely used for managing loading states or tracking specific processes.
 */
export enum LoadingContext {
    Default = '',
    GenerationNumber = 'Generation Number',                           // Loading or calculating generation numbers
    NumberOfPartnerships = 'Number Of Partnerships',                  // Counting or loading partnership information
    NumberOfGenerations = 'Number Of Generations',                    // Calculating total number of generations
    PartnershipToRepresentation = 'Partnership to Representation',    // Converting partnership data to a visual representation
    PersonElementToRepresentation = 'Person Element To Representation', // Converting person data to a visual representation
    ReportChildren = 'Report Children',                               // Generating a report about children
    ReportDeceased = 'Report Deceased',                               // Generating a report about deceased family members
    ReportPartnership = 'Report Partnership',                               // Generating a report about marriages
    RepresentationToPartnership = 'Representation To Partnership',    // Converting visual representation back to partnership data
    RetrieveChildren = 'Retrieve Children',                           // Fetching data about children
    RetrieveClientFamilyName = 'Retrieve Client Family Name',         // Fetching the family name for a client
    RetrieveClientFamilyTree = 'Retrieve Client Family Tree',         // Fetching the entire family tree for a client
    RetrieveClientSelectedPartnership = 'Retrieve Client Selected Partnership', // Fetching a specific partnership selected by the client
    RetrieveClientTitle = 'Retrieve Client Title',                    // Fetching the title for a client
    RetrieveFamilyTree = 'Retrieve Family Tree',                      // Fetching a family tree (possibly not client-specific)
    RetrieveParent = 'Retrieve Parent',                               // Fetching data about a parent
    RevertTree = 'Revert Tree',                                       // Undoing changes to the family tree
    UpdateClientFamilyName = 'Update Client Family Name',             // Updating the family name for a client
    UpdateClientFamilyTree = 'Update Client Family Tree',             // Updating the entire family tree for a client
    UpdateClientSelectedPartnership = 'Update Client Selected Partnership', // Updating a specific partnership selected by the client
    UpdateClientTitle = 'Update Client Title',                        // Updating the title for a client
    ViewSubtree = 'View Subtree'                                      // Loading or viewing a portion of the family tree
}

/**
 * Represents the possible statuses of a Family Tree API response.
 * This is likely used to handle and display the status of API requests.
 */
export enum FamilyTreeApiResponseStatus {
    Failure = 0,     // The operation was not successful
    Processing = -1, // The operation is still in progress
    Success = 1      // The operation completed successfully
}

/**
 * Represents different sections that might appear in a family report.
 * This enum is likely used when generating or displaying family reports.
 */
export enum ReportSections {
    Default,         // A default or overview section
    ReportPartnership,  // A section for reporting marriages
    ReportDeceased,  // A section for listing deceased family members
    ReportChildren   // A section for listing children
};