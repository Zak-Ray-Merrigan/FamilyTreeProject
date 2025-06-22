import React from "react";
import ErrorDisplay from "./ErrorDisplay";
import LoadingDisplay from "./LoadingDisplay";
import useCriticalAttributes from "../hooks/useCriticalAttributes";
import { LoadingContext } from "../Enums";
import { isSuccess } from "../Utils";
import "../styles/Title.css";

const Title: React.FC = () => {
    const {title, titleGetter} = useCriticalAttributes();
    return (
        <>
            <ErrorDisplay response={titleGetter} />
            <LoadingDisplay context={LoadingContext.RetrieveClientTitle} response={titleGetter} />
            {isSuccess(titleGetter) && <><h1 id="title">{title}</h1><br/></>}
        </>
    );
};

export default Title;