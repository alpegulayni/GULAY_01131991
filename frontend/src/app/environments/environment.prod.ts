export const environment = {
  production: true,
  // When deployed behind docker‑compose the frontend should be able to talk
  // to the backend via the same hostname if they share a network.  At
  // runtime the Nginx container will serve the compiled assets, so
  // relative URLs will work.
  apiUrl: ''
};