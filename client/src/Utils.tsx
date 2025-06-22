import FamilyElement from "./models/FamilyElement";
import RepresentationElement from "./models/RepresentationElement";
import MessageResponse from "./models/MessageResponse";
import OutputResponse from "./models/outputResponse";
import PersonElement from "./models/PersonElement";
import ReportDeceasedRequest from "./models/ReportDeceasedRequest";
import ReportChildrenRequest from "./models/ReportChildrenRequest";

export function createURL(path: string, queryParams = {}): string {
    const queryString = new URLSearchParams(queryParams).toString();
    return `${path}?${queryString}`;
}

export async function familyElementToRepresentation(element: FamilyElement): Promise<OutputResponse<RepresentationElement>> {
    const url = 'http://localhost:5201/api/utility/family-element-to-representation';
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: RepresentationElement = await response.json();
    return {output: result};
};

export async function generationNumberOf(element: FamilyElement): Promise<OutputResponse<number>> {
    const url = 'http://localhost:5201/api/familytree/generation-number-of';
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: number = await response.json();
    return {output: result};
}

export async function getFamilies(orderOption: string, memberName: string): Promise<OutputResponse<FamilyElement[]>> {
    const url = createURL(`http://localhost:5201/api/familytree/get-families/${encodeURIComponent(orderOption)}/by-member-name`, {memberName: memberName});
    const response = await fetch(url);
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: FamilyElement[] = await response.json();
    return {output: result};
}

export async function getNumberOfFamilies(): Promise<OutputResponse<number>> {
    const url = 'http://localhost:5201/api/familytree/number-of-families';
    const response = await fetch(url);
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: number = await response.json();
    return {output: result};
}

export async function getNumberOfGenerations(): Promise<OutputResponse<number>> {
    const url = 'http://localhost:5201/api/familytree/number-of-generations';
    const response = await fetch(url);
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: number = await response.json();
    return {output: result};
}

export async function initializeService(familyName: string): Promise<OutputResponse<MessageResponse>> {
    const url = `http://localhost:5201/api/familytree/initialize-service/${familyName}`;
    const response = await fetch(url);
    const result: MessageResponse = await response.json();
    return result.isSuccess ? {output: result} : {problem: result};
}

export async function personElementToRepresentation(element: PersonElement): Promise<OutputResponse<RepresentationElement>> {
    const url = 'http://localhost:5201/api/utility/person-element-to-representation';
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: RepresentationElement = await response.json();
    return {output: result};
}

export async function reportChildren(request: ReportChildrenRequest): Promise<OutputResponse<MessageResponse>> {
    const url = 'http://localhost:5201/api/familytree/report-children';
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    });
    const result: MessageResponse = await response.json();
    return result.isSuccess ? {output: result} : {problem: result};
}

export async function reportDeceased(request: ReportDeceasedRequest): Promise<OutputResponse<MessageResponse>> {
    const url = 'http://localhost:5201/api/familytree/report-deceased';
    const response = await fetch(url, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    });
    const result: MessageResponse = await response.json();
    return result.isSuccess ? {output: result} : {problem: result};
}

export async function reportMarriage(family: FamilyElement): Promise<OutputResponse<MessageResponse>> {
    const url = 'http://localhost:5201/api/familytree/report-married';
    const response = await fetch(url, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(family)
    });
    const result: MessageResponse = await response.json();
    return result.isSuccess ? {output: result} : {problem: result};
}

export async function representationToFamilyElement(representation: RepresentationElement): Promise<OutputResponse<FamilyElement>> {
    const url = 'http://localhost:5201/api/utility/representation-to-family-element';
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(representation)
    });
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: FamilyElement = await response.json();
    return {output: result};
}

export async function retrieveChildren(element: FamilyElement): Promise<OutputResponse<FamilyElement[]>> {
    const url = 'http://localhost:5201/api/familytree/retrieve-children';
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    if (!response.ok) {
        return {problem: await response.json() as MessageResponse};
    }
    return {output: await response.json() as FamilyElement[]};
}

export async function retrieveParent(element: FamilyElement): Promise<OutputResponse<FamilyElement>> {
    const url = `http://localhost:5201/api/familytree/retrieve-parent`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    if (!response.ok) {
        const errorResult: MessageResponse = await response.json();
        return {problem: errorResult};
    }
    const result: FamilyElement = await response.json();
    return {output: result};
}

export async function revertTree(request: File): Promise<OutputResponse<MessageResponse>> {
    const url = `http://localhost:5201/api/familytree/revert-tree`;
    const data = new FormData();
    data.append('file', request);
    const response = await fetch(url, {
        method: 'PUT',
        body: data
    });
    const result: MessageResponse = await response.json();
    return result.isSuccess ? {output: result} : {problem: result};
}

export async function viewSubtree(orderOption: string, memberName: string, family: FamilyElement): Promise<OutputResponse<FamilyElement[]>> {
    const url = createURL(`http://localhost:5201/api/familytree/view-subtree/${encodeURIComponent(orderOption)}/by-member-name`, {memberName: memberName});
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(family)
    });
    return !response.ok ? {problem: await response.json() as MessageResponse} : {output: await response.json() as FamilyElement[]};
}

export const StringDefault = 'unknown';
export const NumberDefault = 0;