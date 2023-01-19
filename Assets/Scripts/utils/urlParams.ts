const urlParams = (params: any): string => {
    const values: string[] = [];

    for (const param of Object.keys(params)) {
        if (Array.isArray(params[param])) {
            params[param].forEach((id: any) => {
                values.push(`${param}=${id}`)
            });
            continue;
        }

        values.push(`${param}=${params[param]}`);
    }

    return values.join('&');
};

export default urlParams;
