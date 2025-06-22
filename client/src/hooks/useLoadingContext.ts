import { useCallback, useState } from "react";
import { LoadingContext } from "../Enums";

const useLoadingContext = () => {
    const [loadingContexts, setLoadingContexts] = useState<LoadingContext[]>([]);
    const addLoadingContext = useCallback((context: LoadingContext) => {
        setLoadingContexts(prevContexts => [...prevContexts, context])
    }, []);

    const removeLoadingContext = useCallback((context: LoadingContext) => {
        setLoadingContexts(prevContexts => prevContexts.filter(c => c !== context));
    }, []);

    const isLoading = useCallback(() => {
        return loadingContexts.length > 0;
    }, [loadingContexts]);

    return {
        isLoading,
        addLoadingContext,
        removeLoadingContext
    };
};

export default useLoadingContext;