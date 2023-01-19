/**
 * Enhances jobs filter to use AJAX to fetch results.
 */

import urlParams from '../../utils/urlParams';
import { analyticsRefreshEvent } from '../analytics/events';

const defaults = {
    pageSize: 12,
};

const CSS = {
    hidden: 'display--none',
};

interface IFetchJobResponse {
    from: number;
    items: string[];
    size: number;
    totalItems: number;
}

const instance = ($el: Element) => {
    const dom = {
        $filters: $el.querySelectorAll('.js-filterable-jobs-filter'),
        $form: $el.querySelector('.js-filterable-jobs-form'),
        $items: $el.querySelector('.js-filterable-jobs-items'),
        $pagerNextBtn: $el.querySelector('.js-filterable-jobs-pager-next-btn'),
        $pagerPreviousBtn: $el.querySelector(
            '.js-filterable-jobs-pager-prev-btn'
        ),
        $pinned: $el.querySelectorAll('.js-job.is-pinned'),
        $submitBtn: $el.querySelector('button[type="submit"]'),
        $total: $el.querySelector('.js-filterable-jobs-total'),
    };

    const getBaseParams = (): any => {
        return {
            query: $el.getAttribute('data-query'),
            size: parseInt(
                $el.getAttribute('data-page-size') ||
                    defaults.pageSize.toString(),
                10
            ),
        };
    };

    const getCurrentPage = (): number => {
        const page = new URLSearchParams(window.location.search).get('page');

        return !page ? 1 : parseInt(page, 10);
    };

    const getFilterParams = (): any => {
        const params: any = {};

        dom.$filters.forEach(($filter) => {
            const filterName = $filter.getAttribute('name');

            if (filterName) {
                params[filterName] = ($filter as HTMLInputElement)?.value;
            }
        });

        return params;
    };

    const getPinnedIds = (): string[] => {
        if (!dom.$pinned) {
            return [];
        }

        const ids: string[] = [];

        dom.$pinned.forEach(($item) => {
            if ($item.getAttribute('data-id') !== null) {
                ids.push($item.getAttribute('data-id') || '');
            }
        });
        return ids;
    };

    const handleInputChange = () => {
        update(
            {
                ...getBaseParams(),
                ...getFilterParams(),
                from: 0,
            },
            true
        );
    };

    const handleUrlChange = () => {
        const params = getBaseParams();

        new URLSearchParams(window.location.search).forEach((value, key) => {
            params[key] = value;
        });

        updateFilters(params);
        update(params, false);
    };

    const hasFilterParams = (): boolean => {
        let hasParam = false;

        dom.$filters.forEach(($filter) => {
            const filterName = $filter.getAttribute('name');

            if (filterName && ($filter as HTMLInputElement)?.value) {
                hasParam = true;
                return false;
            }
        });

        return hasParam;
    };

    const nextPage = (e: Event) => {
        e.preventDefault();

        const baseParams = getBaseParams();

        update(
            {
                ...baseParams,
                ...getFilterParams(),
                from: baseParams.size * getCurrentPage(),
            },
            true
        );

        return false;
    };

    const previousPage = (e: Event) => {
        e.preventDefault();

        const baseParams = getBaseParams();

        update(
            {
                ...baseParams,
                ...getFilterParams(),
                from: baseParams.size * (getCurrentPage() - 2),
            },
            true
        );

        return false;
    };

    const render = (data: IFetchJobResponse) => {
        if (!dom.$items) {
            return;
        }
        dom.$items.innerHTML = '';

        renderPinned(data);

        let total = data.totalItems;

        if (!hasFilterParams() && dom.$pinned) {
            total = data.totalItems + dom.$pinned.length;
        }

        if (dom.$total) {
            dom.$total.innerHTML = total.toString();
        }

        for (const item of data.items) {
            dom.$items.innerHTML += `<div class="content-feed__item">${item}</div>`;
        }

        renderPager(data);

        window.dispatchEvent(new Event(analyticsRefreshEvent));
    };

    const renderPager = (data: IFetchJobResponse) => {
        if (dom.$pagerPreviousBtn) {
            if (data.from === 0) {
                dom.$pagerPreviousBtn.classList.add(CSS.hidden);
            } else {
                dom.$pagerPreviousBtn.classList.remove(CSS.hidden);
            }
        }

        if (dom.$pagerNextBtn) {
            if (data.from + data.size >= data.totalItems) {
                dom.$pagerNextBtn.classList.add(CSS.hidden);
            } else {
                dom.$pagerNextBtn.classList.remove(CSS.hidden);
            }
        }
    };

    const renderPinned = (data: IFetchJobResponse) => {
        if (data.from !== 0 || hasFilterParams()) {
            return;
        }

        dom.$pinned.forEach(($item) => {
            if (dom.$items) {
                dom.$items.appendChild($item);
            }
        });
    };

    const update = (params: any, shouldUpdateUrl: boolean) => {
        if (!params.from) {
            params.from = 0;
        }

        updateParamsForPinned(params);

        window
            .fetch(
                `${
                    $el.getAttribute('data-base-url') || ''
                }/api/greenhouse/jobs?${urlParams(params)}`
            )
            .then((response) => response.json())
            .then(render)
            .finally(() => {
                if (shouldUpdateUrl) {
                    updateUrl(params);
                }
            });
    };

    const updateFilters = (params: any) => {
        dom.$filters.forEach(($filter) => {
            const filterName = $filter.getAttribute('name');

            if (!filterName) {
                return;
            }

            ($filter as HTMLInputElement).value = params[filterName] || '';
        });
    };

    const updateParamsForPinned = (params: any) => {
        if (hasFilterParams()) {
            return params;
        }

        params.excludedIds = getPinnedIds();

        if (params.from !== 0) {
            if (params.excludedIds) {
                params.from -= params.excludedIds.length;
                return params;
            }

            return params;
        }

        params.size -= dom.$pinned.length;

        return params;
    };

    const updateUrl = (params: any) => {
        const ignoredParams = ['from', 'excludedIds', 'size', 'query'];
        let url = `${window.location.origin}${window.location.pathname}?`;

        params.page =
            params.from === 0
                ? 1
                : Math.floor((params.from + dom.$pinned.length) / params.size) +
                  1;

        for (const property in params) {
            if (ignoredParams.indexOf(property) === -1 && params[property]) {
                url += `${property}=${params[property]}&`;
            }
        }

        window.history.pushState(
            '',
            document.title,
            url.substring(0, url.length - 1)
        );
    };

    if (dom.$submitBtn) {
        dom.$submitBtn.remove();
    }

    if (dom.$pagerNextBtn) {
        dom.$pagerNextBtn.addEventListener('click', nextPage);
    }

    if (dom.$pagerPreviousBtn) {
        dom.$pagerPreviousBtn.addEventListener('click', previousPage);
    }

    dom.$filters.forEach(($filter) => {
        $filter.addEventListener('change', handleInputChange);
    });

    window.addEventListener('popstate', handleUrlChange);
};

const filterableJobs = () => {
    const SELECTOR = '.js-filterable-jobs';

    document.querySelectorAll(SELECTOR).forEach(($el) => {
        instance($el);
    });
};

export default filterableJobs;
