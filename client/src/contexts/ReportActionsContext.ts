import React from "react";
import { EmptyResponse } from "../Constants";
import { FamilyTreeApiResponse } from "../Types";

interface ReportActionsContextType {
    reportMade: boolean;
    response: FamilyTreeApiResponse;
    isReportMade: (reportMade: boolean) => void;
    setResponse: (response: FamilyTreeApiResponse) => void;
}

const ReportActionsContext = React.createContext<ReportActionsContextType>({response: EmptyResponse, reportMade: false, isReportMade: () => {}, setResponse: () => {}});

export default ReportActionsContext;