import React, {useEffect, useState} from "react";
import ErrorDisplayComponent from "./ErrorDisplay";
import LoadingComponent from "./LoadingDisplay";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import useLoadingContext from "../hooks/useLoadingContext";
import { getNumberOfPartnerships } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext } from "../Enums";
import { FamilyTreeApiResponse } from "../Types";
import { isProcessing, isSuccess } from "../Utils";

const NumberOfPartnerships: React.FC = () => {
    const {familyName} = useCriticalAttributes();
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [numericOutput, setNumericOutput] = useState<FamilyTreeApiResponse>(EmptyResponse);
    useEffect(() => {
        const fetchNumberOfFamilies = async () => {
            addLoadingContext(LoadingContext.NumberOfPartnerships);
            const response = await getNumberOfPartnerships();
            if (!isProcessing(response)) {
                removeLoadingContext(LoadingContext.NumberOfPartnerships);
            }
            setNumericOutput(response);
        };
        fetchNumberOfFamilies();
    }, [familyName, addLoadingContext, removeLoadingContext]);
    return (
        <>
            <LoadingComponent context={LoadingContext.NumberOfPartnerships} response={numericOutput} />
            <ErrorDisplayComponent response={numericOutput} />
            {isSuccess(numericOutput) && <p>There are {numericOutput.result as number} partnerships in the {familyName} family.</p>}
        </>
    )
};

export default NumberOfPartnerships;