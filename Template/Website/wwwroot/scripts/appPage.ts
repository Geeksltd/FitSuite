import HubPage from 'app/hubPage';


export default class AppPage extends HubPage {

    constructor() {
        super();

        //customizing here 
    }

    public static setDomain() {
        var arrDomain = window.location.hostname.split('.');
        arrDomain.splice(0, 1);
        var domain = arrDomain.join('.');
        document.domain = domain;
    }
}

window["page"] = new HubPage();
