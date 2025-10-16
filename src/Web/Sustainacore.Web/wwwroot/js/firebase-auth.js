import { initializeApp } from "https://www.gstatic.com/firebasejs/10.12.5/firebase-app.js";
import { getAuth, signInWithEmailAndPassword, createUserWithEmailAndPassword, signOut, onAuthStateChanged, getIdToken, getIdTokenResult, updateProfile } from "https://www.gstatic.com/firebasejs/10.12.5/firebase-auth.js";
import { firebaseConfig } from "./firebase-config.js";

const app = initializeApp(firebaseConfig);
export const auth = getAuth(app);

export async function login(email, password) {
    const cred = await signInWithEmailAndPassword(auth, email, password);
    await cred.user.getIdToken(true); // refresh to load latest custom claims
    return cred.user;
}

export async function register(email, password, displayName) {
    const cred = await createUserWithEmailAndPassword(auth, email, password);
    if (displayName) await updateProfile(cred.user, { displayName });
    await cred.user.getIdToken(true);
    return cred.user;
}

export async function currentRole(user) {
    const tokenResult = await getIdTokenResult(user, true);
    return tokenResult.claims.role || "Client";
}

export async function bearer() {
    const user = auth.currentUser;
    if (!user) return null;
    return await getIdToken(user, true);
}

export { onAuthStateChanged, signOut };
