const siteHeader = document.getElementById("siteHeader");

if (siteHeader) {
  siteHeader.innerHTML = `
    <header class="site-header">
      <div class="nav-primary">
        <a href="/" class="site-name">Nick Crandell</a>

        <nav class="page-nav" aria-label="Primary navigation">
          <a href="/work.html">Work</a>
          <a href="/about.html">About</a>
          <a href="https://github.com" target="_blank">GitHub</a>
          <a href="https://linkedin.com/in/nick-crandell-7361a62b4" target="_blank">LinkedIn</a>
          <a href="/assets/nick-crandell-cv.pdf" download>CV</a>
          <a href="/login.html">Login</a>
          <a href="/contact.html">Contact</a>
        </nav>
      </div>
    </header>
  `;
}
