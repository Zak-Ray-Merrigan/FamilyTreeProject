import React from "react";
import {FontAwesomeIcon} from '@fortawesome/react-fontawesome';
import {faArrowUp, faArrowDown} from '@fortawesome/free-solid-svg-icons';

interface ArrowComponentParams {
    isVisible: boolean;
}

const ArrowComponent: React.FC<ArrowComponentParams> = (params) => {
    if (params.isVisible) {
        return (
            <FontAwesomeIcon icon={faArrowUp}/>
        );
    }
    return (
        <FontAwesomeIcon icon={faArrowDown}/>
    );
};

export default ArrowComponent;