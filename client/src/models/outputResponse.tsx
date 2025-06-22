import MessageResponse from "./MessageResponse";
export default interface OutputResponse<T> {
    output?: T,
    problem?: MessageResponse;
}