import { supabase } from './auth.js';

async function requireLogin() {
  const {
    data: { session },
    error
  } = await supabase.auth.getSession();

  if (error || !session) {
    window.location.replace('/login.html');
  }
}

requireLogin();
