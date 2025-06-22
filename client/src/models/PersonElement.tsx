import { StringDefault } from "../Utils";

export default interface PersonElement {
    name: string;
    birthDate: string;
    deceasedDate: string;
}

export const PersonDefault: PersonElement = {
    name: StringDefault,
    birthDate: StringDefault,
    deceasedDate: StringDefault
};