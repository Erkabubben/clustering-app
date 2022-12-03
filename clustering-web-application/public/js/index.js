async function button0() {
    const responseJSON = await fetchRequest("./findTopMatchingUsers")
    removeTableChildren()
    addRowToTable(resultsTable, ['User', 'Similarity'], true)
    for (let i = 0; i < responseJSON.users.length; i++) {
        addRowToTable(resultsTable, [responseJSON.users[i], responseJSON.similarities[i]], false)
    }
}

async function button1() {
    const responseJSON = await fetchRequest("./findMovieRecommendationsForUser")
    removeTableChildren()
    addRowToTable(resultsTable, ['Movie', 'ID', 'Score'], true)
    for (let i = 0; i < responseJSON.movies.length; i++) {
        addRowToTable(resultsTable, [responseJSON.movies[i], responseJSON.ids[i], responseJSON.scores[i]], false)
    }
}

async function button2() {
    const responseJSON = await fetchRequest("./findMovieRecommendationsItemBased")
    removeTableChildren()
    addRowToTable(resultsTable, ['Movie', 'ID', 'Score'], true)
    for (let i = 0; i < responseJSON.movies.length; i++) {
        addRowToTable(resultsTable, [responseJSON.movies[i], responseJSON.ids[i], responseJSON.scores[i]], false)
    }
}

async function fetchRequest (url) {
    const formDataString = await JSON.stringify(getFormData())
    const response = await fetch(url, {
        method: 'post',
        body: formDataString,
        headers: { 'Content-Type': 'application/json' }
    })
    return await response.json()
}

function removeTableChildren() {
    resultsTable.removeAttribute('hidden')
    while (resultsTable.lastChild !== null) {
        resultsTable.removeChild(resultsTable.lastChild)
    }
}

function addRowToTable (table, textContent, isHeader) {
    const row = document.createElement('tr')
    textContent.forEach(element => {
        const newElement = isHeader ? document.createElement('th') : document.createElement('td')
        newElement.textContent = element
        row.appendChild(newElement)
    })
    table.appendChild(row)
}

function getFormData() {
    return {
        user: document.querySelector('#users').value,
        similarity: document.querySelector('#similarity').value,
        results: document.querySelector('#results').value
    }
}

var resultsTable = document.querySelector('#results-table')
document.querySelector('#button-find-matching-users').addEventListener('click', button0)
document.querySelector('#button-find-movies').addEventListener('click', button1)
document.querySelector('#button-find-movies-item').addEventListener('click', button2)
