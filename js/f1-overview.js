const raceStrip = document.getElementById('raceStrip');

const countryFlags = {
  Australia: '🇦🇺',
  China: '🇨🇳',
  Japan: '🇯🇵',
  Bahrain: '🇧🇭',
  'Saudi Arabia': '🇸🇦',
  USA: '🇺🇸',
  Italy: '🇮🇹',
  Monaco: '🇲🇨',
  Spain: '🇪🇸',
  Canada: '🇨🇦',
  Austria: '🇦🇹',
  UK: '🇬🇧',
  Hungary: '🇭🇺',
  Belgium: '🇧🇪',
  Netherlands: '🇳🇱',
  Azerbaijan: '🇦🇿',
  Singapore: '🇸🇬',
  Mexico: '🇲🇽',
  Brazil: '🇧🇷',
  Qatar: '🇶🇦',
  UAE: '🇦🇪'
};

function formatRaceDate(dateString) {
  const date = new Date(`${dateString}T12:00:00Z`);
  const day = date.getUTCDate();
  const month = date.getUTCMonth() + 1;

  return `${day}/${month}`;
}

function getNextRaceIndex(races) {
  const today = new Date();

  return races.findIndex((race) => {
    const raceDate = new Date(`${race.date}T23:59:59Z`);
    return raceDate >= today;
  });
}

async function loadRaceStrip() {
  try {
    const response = await fetch('https://api.jolpi.ca/ergast/f1/2026.json');

    if (!response.ok) {
      throw new Error('Unable to load race calendar.');
    }

    const data = await response.json();
    const races = data.MRData.RaceTable.Races;
    const nextRaceIndex = getNextRaceIndex(races);

    raceStrip.innerHTML = races.map((race, index) => {
      const country = race.Circuit.Location.country;
      const flag = countryFlags[country] || '🏳️';
      const date = formatRaceDate(race.date);
      const isNext = index === nextRaceIndex;

      return `
        <div class="race-marker" title="${race.raceName}">
          <div class="race-flag">${flag}</div>
          <div class="race-date">${date}</div>
          <div class="race-dot ${isNext ? 'is-next' : ''}"></div>
        </div>
      `;
    }).join('');
  } catch (error) {
    raceStrip.innerHTML = '<p class="f1-error">Unable to load race calendar.</p>';
    console.error(error);
  }
}

loadRaceStrip();
