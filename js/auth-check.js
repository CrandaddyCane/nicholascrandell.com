import { supabase } from './auth.js';

const {
  data: { user },
  error
} = await supabase.auth.getUser();

if (error || !user) {
  window.location.href = '/login.html';
}

