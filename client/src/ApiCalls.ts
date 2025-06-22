/**
 * ApiCalls.ts
 * 
 * This file contains functions for making API calls to the Family Tree and Utility services.
 * These functions handle various operations such as retrieving family tree data, 
 * updating client information, and performing family tree manipulations.
 */

import _ from "lodash";
import { FamilyTreeApiResponse, Partnership, Person, ReportChildrenRequest, ReportDeceasedRequest, ReportMarriageRequest, RepresentationElement } from "./Types";
import { UtilityController, FamilyTreeController} from "./Constants";
import { createURL } from "./Utils";

/**
 * Retrieves the generation number of a given partnership.
 * @param element - The partnership to get the generation number for
 * @returns A promise resolving to the API response
 */
export async function generationNumberOf(element: Partnership): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/generation-number-of`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Retrieves the client's family name from the server.
 * @returns A promise resolving to the API response
 */
export async function getClientFamilyName(): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/get-client-family-name`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Retrieves the client's family tree from the server.
 * @returns A promise resolving to the API response
 */
export async function getClientFamilyTree(): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/get-client-family-tree`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Retrieves the client's selected partnership from the server.
 * @returns A promise resolving to the API response
 */
export async function getClientSelectedPartnership(): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/get-client-selected-partnership`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Retrieves the client's title from the server.
 * @returns A promise resolving to the API response
 */
export async function getClientTitle(): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/get-client-title`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Retrieves the total number of partnerships in the family tree.
 * @returns A promise resolving to the API response
 */
export async function getNumberOfPartnerships(): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/number-of-partnerships`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Retrieves the total number of generations in the family tree.
 * @returns A promise resolving to the API response
 */
export async function getNumberOfGenerations(): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/number-of-generations`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Retrieves partnerships based on a member's name and an order option.
 * @param orderOption - The order in which to retrieve partnerships
 * @param memberName - The name of the member to retrieve partnerships for
 * @returns A promise resolving to the API response
 */
export async function getPartnerships(orderOption: string, memberName: string): Promise<FamilyTreeApiResponse> {
    const url = createURL(`${FamilyTreeController}/get-partnerships/${encodeURIComponent(orderOption)}/by-member-name`, {memberName: memberName});
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Initializes the family tree service with a given family name.
 * @param familyName - The name of the family to initialize
 * @returns A promise resolving to the API response
 */
export async function initializeService(familyName: string): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/initialize-service/${familyName}`;
    return await (await fetch(url)).json() as FamilyTreeApiResponse;
}

/**
 * Converts a partnership to its representation.
 * @param partnership - The partnership to convert
 * @returns A promise resolving to the API response
 */
export async function partnershipToRepresentation(partnership: Partnership): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/partnership-to-representation`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(partnership)
    });
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Converts a person to their representation.
 * @param person - The person to convert
 * @returns A promise resolving to the API response
 */
export async function personToRepresentation(person: Person): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/person-to-representation`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(person)
    });
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Reports children for a given partnership.
 * @param request - The request containing parent and child partnership information
 * @returns A promise resolving to the API response
 */
export async function reportChildren(request: ReportChildrenRequest): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/report-children`;
    return await (await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    })).json() as FamilyTreeApiResponse;
}

/**
 * Reports a person as deceased.
 * @param request - The request containing the person and their deceased date
 * @returns A promise resolving to the API response
 */
export async function reportDeceased(request: ReportDeceasedRequest): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/report-deceased`;
    return await (await fetch(url, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    })).json() as FamilyTreeApiResponse;
}

/**
 * Reports a marriage.
 * @param request - The request containing the initial member and the new family partnership
 * @returns A promise resolving to the API response
 */
export async function reportMarriage(request: ReportMarriageRequest): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/report-married`;
    return await (await fetch(url, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    })).json() as FamilyTreeApiResponse;
}

/**
 * Converts a representation to a partnership.
 * @param representation - The representation to convert
 * @returns A promise resolving to the API response
 */
export async function representationToPartnership(representation: RepresentationElement): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/representation-to-partnership`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(representation)
    });
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Retrieves children for a given partnership.
 * @param element - The partnership to retrieve children for
 * @returns A promise resolving to the API response
 */
export async function retrieveChildren(element: Partnership): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/retrieve-children`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Retrieves the parent for a given partnership.
 * @param element - The partnership to retrieve the parent for
 * @returns A promise resolving to the API response
 */
export async function retrieveParent(element: Partnership): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/retrieve-parent`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Reverts the family tree to a previous state.
 * @param request - The file containing the previous state
 * @returns A promise resolving to the API response
 */
export async function revertTree(request: File): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/revert-tree`;
    const data = new FormData();
    data.append('file', request);
    const response = await fetch(url, {
        method: 'PUT',
        body: data
    });
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Sets the client's family name on the server.
 * @param familyName - The new family name (or null to clear)
 * @returns A promise resolving to the API response
 */
export async function setClientFamilyName(familyName: string | null): Promise<FamilyTreeApiResponse> {
    const url = _.isNull(familyName) ? `${UtilityController}/set-client-family-name` : `${UtilityController}/set-client-family-name/${encodeURIComponent(familyName)}`;
    const response = await fetch(url, {
        method: 'PUT'
    });
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Sets the client's family tree on the server.
 * @param familyTree - The new family tree
 * @returns A promise resolving to the API response
 */
export async function setClientFamilyTree(familyTree: Partnership[]): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/set-client-family-tree`;
    const response = await fetch(url, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(familyTree)
    });
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Sets the client's selected partnership on the server.
 * @param selectedPartnership - The new selected partnership
 * @returns A promise resolving to the API response
 */
export async function setClientSelectedPartnership(selectedPartnership: Partnership): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/set-client-selected-partnership`;
    const response = await fetch(url, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(selectedPartnership)
    });
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Sets the client's title on the server.
 * @param title - The new title
 * @returns A promise resolving to the API response
 */
export async function setClientTitle(title: string): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/set-client-title/${encodeURIComponent(title)}`;
    const response = await fetch(url, {
        method: 'PUT'
    });
    return await response.json() as FamilyTreeApiResponse;
}

/**
 * Retrieves a subtree of the family tree based on a specific member and partnership.
 * @param orderOption - The order in which to retrieve the subtree
 * @param memberName - The name of the member to start the subtree from
 * @param partnership - The partnership to start the subtree from
 * @returns A promise resolving to the API response
 */
export async function viewSubtree(orderOption: string, memberName: string, partnership: Partnership): Promise<FamilyTreeApiResponse> {
    const url = createURL(`${FamilyTreeController}/view-subtree/${encodeURIComponent(orderOption)}/by-member-name`, {memberName: memberName});
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(partnership)
    });
    return await response.json() as FamilyTreeApiResponse;
}