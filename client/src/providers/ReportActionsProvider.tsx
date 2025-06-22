import React, { useState } from "react";
import ReportActionsContext from "../models/ReportActionsContext";
import { ProviderProps } from "../models/providerProps";
import OutputResponse from "../models/outputResponse";
import MessageResponse from "../models/MessageResponse";

const ReportActionsProvider: React.FC<ProviderProps> = ({children}) => {
    const [response, setResponse] = useState<OutputResponse<MessageResponse>>({});
    return (
        <ReportActionsContext.Provider value={{response: response, setResponse: setResponse}}>
            {children}
        </ReportActionsContext.Provider>
    );
};

export default ReportActionsProvider;