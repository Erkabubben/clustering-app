async function button0() {
    const responseJSON = await fetchRequest("./KMeansClustering")
    /*removeTableChildren()
    addRowToTable(resultsTable, ['User', 'Similarity'], true)
    for (let i = 0; i < responseJSON.users.length; i++) {
        addRowToTable(resultsTable, [responseJSON.users[i], responseJSON.similarities[i]], false)
    }*/
}

async function button1() {
    const responseJSON = await fetchRequest("./HierarchichalClustering")
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
    return {}
}

var resultsTable = document.querySelector('#results-table')
document.querySelector('#button-k-means-clustering').addEventListener('click', button0)
document.querySelector('#button-hierarchical-clustering').addEventListener('click', button1)
