import React from "react";
import OutputResponse from "./outputResponse";
import MessageResponse from "./MessageResponse";

interface ReportActionsContextType {
    response: OutputResponse<MessageResponse>;
    setResponse: (response: OutputResponse<MessageResponse>) => void;
}

const ReportActionsContext = React.createContext<ReportActionsContextType>({response: {}, setResponse: () => {}});

export default ReportActionsContext;