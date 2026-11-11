export const getToken = () => typeof window !== "undefined" ? localStorage.getItem("bq_token") : null;

export const setToken = (t: string) => localStorage.setItem("bq_token", t);

export const clearToken = () => localStorage.removeItem("bq_token");