import PersonElement from "./PersonElement";
export enum PersonType {
    Member = 'Member',
    InLaw = 'InLaw'
};

export default interface PersonInfoInput {
    type: PersonType;
    element: PersonElement
}