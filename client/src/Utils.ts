/**
 * Utils.ts
 * 
 * This file contains utility functions used throughout the family tree application.
 * These functions handle common tasks such as URL creation, loading text generation,
 * and API response status checking.
 */

import _ from "lodash";
import { FamilyTreeApiResponseStatus, LoadingContext } from "./Enums";
import { EmptyResponse } from "./Constants";
import { FamilyTreeApiResponse } from "./Types";

/**
 * Creates a URL with query parameters.
 * @param path - The base URL path
 * @param queryParams - An object containing query parameters
 * @returns A string representing the full URL with query parameters
 */
export function createURL(path: string, queryParams = {}): string {
    const queryString = new URLSearchParams(queryParams).toString();
    return `${path}?${queryString}`;
}

/**
 * Generates a loading text based on the current loading context.
 * @param context - The current loading context
 * @returns A string describing the current loading operation
 */
export function getLoadingText(context: LoadingContext): string {
    // Special case for representation-related contexts
    if (context === LoadingContext.PartnershipToRepresentation || context === LoadingContext.PersonElementToRepresentation) {
        return 'Retrieving Representation...';
    }
    
    // Switch statement for other loading contexts
    switch (context) {
        case LoadingContext.GenerationNumber: return 'Retrieving Generation Number...';
        case LoadingContext.NumberOfPartnerships: return 'Retrieving Number Of Families...';
        case LoadingContext.NumberOfGenerations: return 'Retrieving Number Of Generations...';
        case LoadingContext.ReportChildren: return 'Children Report is Processing...';
        case LoadingContext.ReportDeceased: return 'Deceased Report is Processing...';
        case LoadingContext.ReportMarriage: return 'Marriage Report is Processing...';
        case LoadingContext.RepresentationToPartnership: return 'Updating Selection Of Family Element...';
        case LoadingContext.RetrieveChildren: return 'Retrieving Children...';
        case LoadingContext.RetrieveClientFamilyName: return 'Retrieving Backup Family Name From Server...';
        case LoadingContext.RetrieveClientFamilyTree: return 'Retrieving Backup Family Tree From Server...';
        case LoadingContext.RetrieveClientSelectedPartnership: return 'Retrieving Backup Selected Partnership From Server...';
        case LoadingContext.RetrieveClientTitle: return 'Retrieving Backup Title From Server...';
        case LoadingContext.RetrieveFamilyTree: return 'Retrieving Family Tree...';
        case LoadingContext.RetrieveParent: return 'Retrieving Parent...';
        case LoadingContext.RevertTree: return 'Reverting Family Tree...';
        case LoadingContext.UpdateClientFamilyName: return 'Updating and Backing up updated family name...';
        case LoadingContext.UpdateClientFamilyTree: return 'Updating and Backing up updated family tree...';
        case LoadingContext.UpdateClientSelectedPartnership: return 'Updating and Backing up updated selected partnership...';
        case LoadingContext.UpdateClientTitle: return 'Updating and Backing up updated page title...';
        case LoadingContext.ViewSubtree: return 'Retrieving Subtree...';
    }
    
    // Default loading text if context is not recognized
    return 'Loading Content...';
}

/**
 * Checks if an API response indicates that the operation is still processing.
 * @param response - The API response to check
 * @returns True if the response matches the EmptyResponse (indicating processing), false otherwise
 */
export function isProcessing(response: FamilyTreeApiResponse) {
    return _.isEqual(response, EmptyResponse);
}

/**
 * Checks if an API response indicates a successful operation.
 * @param response - The API response to check
 * @returns True if the response status is Success, false otherwise
 */
export function isSuccess(response: FamilyTreeApiResponse) {
    return response.status === FamilyTreeApiResponseStatus.Success;
}