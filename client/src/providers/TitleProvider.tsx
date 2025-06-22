import React, { useState } from "react";
import TitleContext from "../models/TitleContext";
import { ProviderProps } from "../models/providerProps";

const TitleProvider: React.FC<ProviderProps> = ({children}) => {
    const [title, setTitle] = useState<string>('');

    return (
        <TitleContext.Provider value={{title: title, setTitle: setTitle}}>
            {children}
        </TitleContext.Provider>
    );
};

export default TitleProvider;