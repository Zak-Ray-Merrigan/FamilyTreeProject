/**
 * Constants.ts
 * 
 * This file defines various constants used throughout the family tree application.
 * These constants provide default values, API endpoints, and other configuration settings.
 */

import { FamilyTreeApiResponseStatus } from "./Enums";
import { FamilyTreeApiResponse, Partnership } from "./Types";

/**
 * Default empty response for API calls.
 * Used as an initial state or when no data is available.
 */
export const EmptyResponse: FamilyTreeApiResponse = {
    status: FamilyTreeApiResponseStatus.Processing, // Set to Processing by default
    message: ''                                     // Empty message
}

/**
 * Base URL for the Family Tree API controller.
 * Note: This is set to localhost and should be updated for production environments.
 */
export const FamilyTreeController = 'http://localhost:5201/api/familytree';

/**
 * Initial title displayed in the application.
 */
export const InitialTitle = 'Welcome to the Client-Side of my Family Tree Project';

/**
 * Default number value used in the application.
 */
export const NumberDefault = 0;

/**
 * Represents the root of the family tree.
 */
export const Root: Partnership = {
    member: null,         // No member
    inLaw: null,          // No in-law
    partnershipDate: null // No partnership date
};

/**
 * Base URL for the Utility API controller.
 * Note: This is set to localhost and should be updated for production environments.
 */
export const UtilityController = 'http://localhost:5201/api/utility';