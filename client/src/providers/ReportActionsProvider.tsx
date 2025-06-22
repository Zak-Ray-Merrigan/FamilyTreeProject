import React, { useState } from "react";
import ReportActionsContext from "../contexts/ReportActionsContext";
import { EmptyResponse } from "../Constants";
import { FamilyTreeApiResponse, ProviderProps } from "../Types";

const ReportActionsProvider: React.FC<ProviderProps> = ({children}) => {
    const [response, setResponse] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [reportMade, isReportMade] = useState<boolean>(false);
    return (
        <ReportActionsContext.Provider value={{response, reportMade, isReportMade, setResponse}}>
            {children}
        </ReportActionsContext.Provider>
    );
};

export default ReportActionsProvider;