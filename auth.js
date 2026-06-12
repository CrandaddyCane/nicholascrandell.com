import { createClient } from 'https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2/+esm';

const SUPABASE_URL = 'https://yrhumnkgemvjmeqqqqzx.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InlyaHVtbmtnZW12am1lcXFxcXp4Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3ODEyMzAyNDUsImV4cCI6MjA5NjgwNjI0NX0.7QcJ4vLnhR3sg2xSGOZs2lgRfjQ0HnW1EsmPZs9264o';

export const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY);
