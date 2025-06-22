import FamilyElement from "./FamilyElement";

export default interface ReportChildrenRequest {
    parent: FamilyElement;
    child: FamilyElement;
};