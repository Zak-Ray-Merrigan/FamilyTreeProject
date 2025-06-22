import PersonElement from "./PersonElement";

export default interface ReportDeceasedRequest {
    element: PersonElement;
    deceasedDate: string;
};