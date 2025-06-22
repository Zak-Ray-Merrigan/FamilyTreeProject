import React, { FormEvent, useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import TitleContext from "../models/TitleContext";
import OutputResponse from "../models/outputResponse";
import MessageResponse from "../models/MessageResponse";
import { initializeService } from "../Utils";
import FamilyNameContext from "../models/familyNameContext";
import _ from "lodash";
import ErrorDisplayComponent from "../components/ErrorDisplayComponent";


const ChooseFamilyNamePage: React.FC = () => {
    let navigate = useNavigate();
    const {familyName, setFamilyName} = useContext(FamilyNameContext);
    const {setTitle} = useContext(TitleContext);
    const [familyNameResponse, setFamilyNameResponse] = useState<OutputResponse<MessageResponse>>({});
    const handleSubmitFamilyName = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (!_.isEqual(familyName, '')) {
            const response: OutputResponse<MessageResponse> = await initializeService(familyName);
            if (response.output) {
                setTitle(response.output.message);
                navigate('/family-tree');
            }
        }
        else {
            setFamilyNameResponse({problem: {message: 'No name has been provided.', isSuccess: false}});
        }
    };
    return (
        <form onSubmit={handleSubmitFamilyName}>
            <h1>Welcome to the Client-Side of my Family Tree Project</h1><br/>
            <label>Enter a Family Name:&nbsp;<input type="text" value={familyName} onChange={(e) => setFamilyName(e.target.value)}/></label><br/>
            <button type="submit">Go To Family Tree</button>
            {!_.isUndefined(familyNameResponse.problem) && <ErrorDisplayComponent message={familyNameResponse.problem.message}/>}
        </form>
    );
};
export default ChooseFamilyNamePage;