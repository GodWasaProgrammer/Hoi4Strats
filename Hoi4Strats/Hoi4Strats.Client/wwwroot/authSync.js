console.log("authSync.js laddas...");
window.authSync = {
    getCookie: (name) => {
        console.log("Försöker hämta cookie:", name);
        const value = document.cookie.match(`(^|;)\\s*${name}\\s*=\\s*([^;]+)`);
        return value ? decodeURIComponent(value[2]) : null;
    },
    deleteCookie: (name) => {
        console.log("Rensar cookie:", name);
        document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
    }
};