window.authSync = {
    getCookie: (name) => {
        const value = document.cookie.match(`(^|;)\\s*${name}\\s*=\\s*([^;]+)`);
        return value ? decodeURIComponent(value[2]) : null;
    },
    deleteCookie: (name) => {
        document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
    }
};