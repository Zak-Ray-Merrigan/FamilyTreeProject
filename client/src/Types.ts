/**
 * Types.ts
 * 
 * This file defines various TypeScript interfaces used throughout the family tree application.
 * These interfaces provide type safety and structure for the data used in the application.
 */

import { ReactNode } from "react";
import { FamilyTreeApiResponseStatus } from "./Enums";

/**
 * Represents the structure of an API response for family tree operations.
 */
export interface FamilyTreeApiResponse {
    status: FamilyTreeApiResponseStatus; // The status of the API response (Success, Failure, or Processing)
    message: string;                     // A message describing the result of the operation
    result?: any;                        // Optional field for any additional data returned by the API
}

/**
 * Represents a person in the family tree.
 */
export interface Person {
    name: string;                // The person's name
    birthDate: string;           // The person's birth date
    deceasedDate: string | null; // The person's date of death, if applicable. Null if the person is alive.
}

/**
 * Represents a partnership (e.g., marriage) in the family tree.
 */
export interface Partnership {
    member: Person | null;        // The family member in the partnership
    inLaw: Person | null;         // The person who married into the family
    partnershipDate: string | null; // The date of the partnership (e.g., wedding date), null if unknown
}

/**
 * Props for React context providers.
 */
export interface ProviderProps {
    children: ReactNode; // The child components wrapped by the provider
}

/**
 * Request structure for reporting children in the family tree.
 */
export interface ReportChildrenRequest {
    parent: Partnership; // The partnership representing the parents
    child: Partnership;  // The partnership representing the child and their spouse (if any)
}

/**
 * Request structure for reporting a deceased person in the family tree.
 */
export interface ReportDeceasedRequest {
    person: Person;     // The person who has passed away
    deceasedDate: string; // The date of death
}

/**
 * Request structure for reporting a partnership in the family tree.
 */
export interface ReportPartnershipRequest {
    initialMember: Person; // The original family member having an additional partnership
    newPartnership: Partnership;   // The new partnership formed by the marriage
}

/**
 * Represents an element in the visual representation of the family tree.
 */
export interface RepresentationElement {
    representation: string; // A string representation of the element (could be an ID or a visual descriptor)
}