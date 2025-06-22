import React, {useContext, useEffect, useState} from "react";
import FamilyNameContext from "../models/familyNameContext";
import OutputResponse from "../models/outputResponse";
import { NumberDefault, getNumberOfGenerations } from "../Utils";

const GetNumberOfGenerationsComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const [numericOutput, setNumericOutput] = useState<OutputResponse<number>>({});
    useEffect(() => {
        const fetchNumberOfGenerations = async () => {
            const response: OutputResponse<number> = await getNumberOfGenerations();
            setNumericOutput(response);
        };
        fetchNumberOfGenerations();
    }, [familyName]);
    return (
        <p>There are {numericOutput.output ? NumberDefault : numericOutput.output} generations in the {familyName} family tree.</p>
    )
}

export default GetNumberOfGenerationsComponent;