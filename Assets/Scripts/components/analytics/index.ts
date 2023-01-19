/**
 * Custom analytics tracking.
 */

import { analyticsRefreshEvent } from './events';
import addToCart from './tracking/addToCart';
import click from './tracking/click';
import detail from './tracking/detail';
import impression from './tracking/impression';
import purchase from './tracking/purchase';
import purchaseClick from './tracking/purchaseClick';

declare global {
    interface Window {
        dataLayer: any[];
        google_tag_manager: any;
        gtag: Gtag.Gtag;
    }
}

const SELECTOR = '.js-analytics-track';

const getEvents = ($el: Element) => {
    return $el
        .getAttribute('data-events')
        ?.split(',')
        .map((event) => event.trim().toLowerCase());
};

const instance = ($el: Element) => {
    (getEvents($el) || []).map((event) => {
        switch (event) {
            case 'addtocart':
                addToCart($el);
                break;
            case 'click':
                click($el);
                break;
            case 'detail':
                detail($el);
                break;
            case 'impression':
                impression($el);
                break;
            case 'purchase':
                purchase($el);
                break;
            case 'purchaseclick':
                purchaseClick($el);
                break;
            default:
                break;
        }
    });
};

const instanceClickOrImpression = ($el: Element) => {
    (getEvents($el) || []).map((event) => {
        switch (event) {
            case 'click':
                click($el);
                break;
            case 'impression':
                impression($el);
                break;
            default:
                break;
        }
    });
};

const refresh = () => {
    document.querySelectorAll(SELECTOR).forEach(($el: Element) => {
        instanceClickOrImpression($el);
    });
};

const analytics = () => {
    window.dataLayer = window.dataLayer || [];

    document.querySelectorAll(SELECTOR).forEach(($el: Element) => {
        instance($el);
    });

    window.addEventListener(analyticsRefreshEvent, refresh);
};

export default analytics;
