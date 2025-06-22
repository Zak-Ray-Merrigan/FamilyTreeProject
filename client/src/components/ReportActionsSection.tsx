import React, { useContext, useState } from "react";
import ReportActionsContext from "../models/ReportActionsContext";
import ReportActionsProvider from "../providers/ReportActionsProvider";
import './ReportActionsSection.css';
import ReportMarriageForm from "./ReportMarriageForm";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import ReportDeceasedForm from "./ReportDeceasedForm";
import ReportChildrenForm from "./ReportChildrenForm";

enum Sections {
    Default,
    ReportMarriage,
    ReportDeceased,
    ReportChildren
};

const ReportActionsSection: React.FC = () => {
    const [selectedSection, changeSelectedSection] = useState<Sections>(Sections.Default);
    const {response} = useContext(ReportActionsContext);

    return (
        <ReportActionsProvider>
            <h2>Report Actions:</h2>
            <section id="report-actions-section">
                <div id="report-action-headers">
                    <header className="report-action-header" onClick={() => changeSelectedSection(Sections.ReportMarriage)} tabIndex={0} role="button">
                        Report Marriage
                    </header>
                    <header className="report-action-header" onClick={() => changeSelectedSection(Sections.ReportDeceased)} tabIndex={0} role="button">
                        Report Deceased
                    </header>
                    <header className="report-action-header" onClick={() => changeSelectedSection(Sections.ReportChildren)} tabIndex={0} role="button">
                        Report Children
                    </header>
                </div>
                <div id="report-action-form">
                    {selectedSection === Sections.ReportMarriage && <ReportMarriageForm />}
                    {selectedSection === Sections.ReportDeceased && <ReportDeceasedForm />}
                    {selectedSection === Sections.ReportChildren && <ReportChildrenForm />}
                </div>
                {response.problem && <ErrorDisplayComponent message={response.problem.message}/>}
            </section>
        </ReportActionsProvider>
    );
};

export default ReportActionsSection;