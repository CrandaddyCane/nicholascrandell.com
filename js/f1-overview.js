const raceStrip = document.getElementById('raceStrip');
const driverStandingsBody = document.getElementById('driverStandingsBody');

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

const nationalityFlags = {
  Australian: '🇦🇺',
  British: '🇬🇧',
  Dutch: '🇳🇱',
  Monegasque: '🇲🇨',
  German: '🇩🇪',
  Spanish: '🇪🇸',
  French: '🇫🇷',
  Italian: '🇮🇹',
  Finnish: '🇫🇮',
  Mexican: '🇲🇽',
  Canadian: '🇨🇦',
  Japanese: '🇯🇵',
  Thai: '🇹🇭',
  Chinese: '🇨🇳',
  Brazilian: '🇧🇷',
  Argentine: '🇦🇷',
  American: '🇺🇸',
  Danish: '🇩🇰',
  'New Zealande'r: '🇳🇿'
};

const cancelledRaces = [
  {
    raceName: 'Bahrain Grand Prix',
    date: '2026-04-12',
    country: 'Bahrain',
    cancelled: true
  },
  {
    raceName: 'Saudi Arabian Grand Prix',
    date: '2026-04-19',
    country: 'Saudi Arabia',
    cancelled: true
  }
];

function formatRaceDate(dateString) {
  const date = new Date(`${dateString}T12:00:00Z`);
  const month = date.getUTCMonth() + 1;
  const day = date.getUTCDate();

  return `${month}/${day}`;
}

function getNextRaceIndex(races) {
  const today = new Date();

  return races.findIndex((race) => {
    if (race.cancelled) return false;

    const raceDate = new Date(`${race.date}T23:59:59Z`);
    return raceDate >= today;
  });
}

function normalizeRace(race) {
  return {
    raceName: race.raceName,
    date: race.date,
    country: race.Circuit.Location.country,
    cancelled: false
  };
}

async function loadRaceStrip() {
  try {
    const response = await fetch('https://api.jolpi.ca/ergast/f1/2026.json');

    if (!response.ok) {
      throw new Error('Unable to load race calendar.');
    }

    const data = await response.json();
    const apiRaces = data.MRData.RaceTable.Races.map(normalizeRace);

    const races = [...apiRaces, ...cancelledRaces].sort((a, b) => {
      return new Date(a.date) - new Date(b.date);
    });

    const nextRaceIndex = getNextRaceIndex(races);

    raceStrip.innerHTML = races.map((race, index) => {
      const flag = countryFlags[race.country] || '';
      const date = formatRaceDate(race.date);
      const isNext = index === nextRaceIndex;
      const isPast = new Date(`${race.date}T23:59:59Z`) < new Date();

      return `
        <div class="race-marker ${isPast ? 'is-past' : 'is-future'} ${race.cancelled ? 'is-cancelled' : ''}" title="${race.raceName}">
          <div class="race-flag-wrap">
            <div class="race-flag">${flag}</div>
            ${race.cancelled ? '<div class="race-cancelled-x">×</div>' : ''}
          </div>
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

async function loadDriverStandings() {
  try {
    const response = await fetch('https://api.jolpi.ca/ergast/f1/2026/driverstandings.json');

    if (!response.ok) {
      throw new Error('Unable to load driver standings.');
    }

    const data = await response.json();
    const standingsList = data.MRData.StandingsTable.StandingsLists[0];

    if (!standingsList) {
      driverStandingsBody.innerHTML = `
        <div class="standings-loading">
          No driver standings available yet.
        </div>
      `;
      return;
    }

    const standings = standingsList.DriverStandings;

    driverStandingsBody.innerHTML = standings.map((standing) => {
      const driver = standing.Driver;
      const team = standing.Constructors?.[0] || standing.Constructor || {};
      const flag = nationalityFlags[driver.nationality] || '';

      return `
        <div class="standings-row">
          <div>${standing.position}</div>
          <div class="change-cell">–</div>
          <div class="driver-cell">
            <span>${flag}</span>
            <span>${driver.familyName}</span>
          </div>
          <div>${team.name || 'Unknown'}</div>
          <div class="points-cell">${standing.points}</div>
        </div>
      `;
    }).join('');
  } catch (error) {
    driverStandingsBody.innerHTML = `
      <div class="standings-loading">
        Unable to load driver standings.
      </div>
    `;
    console.error(error);
  }
}

loadRaceStrip();
loadDriverStandings();
