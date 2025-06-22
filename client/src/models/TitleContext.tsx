import React from "react";
interface TitleContextType {
    title: string;
    setTitle: (title: string) => void;
}

const TitleContext = React.createContext<TitleContextType>({title: '', setTitle: () => {}});

export default TitleContext;