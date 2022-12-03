async function button0() {
    const responseJSON = await getFetchRequest("./KMeansClustering")
    updateTreeViewFromKMeansData(responseJSON)
    /*removeTableChildren()
    addRowToTable(resultsTable, ['User', 'Similarity'], true)
    for (let i = 0; i < responseJSON.users.length; i++) {
        addRowToTable(resultsTable, [responseJSON.users[i], responseJSON.similarities[i]], false)
    }*/
}

async function button1() {
    const responseJSON = await getFetchRequest("./HierarchichalClustering")
    removeTableChildren()
    addRowToTable(resultsTable, ['Movie', 'ID', 'Score'], true)
    for (let i = 0; i < responseJSON.movies.length; i++) {
        addRowToTable(resultsTable, [responseJSON.movies[i], responseJSON.ids[i], responseJSON.scores[i]], false)
    }
}

async function getFetchRequest (url) {
    const formDataString = await JSON.stringify(getFormData())
    const response = await fetch(url, {
        method: 'get',
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

function removeChildren(node) {
    while (node.lastChild !== null) {
        node.removeChild(node.lastChild)
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

function updateTreeViewTogglers() {
    var toggler = document.getElementsByClassName("caret");
    var i;

    for (i = 0; i < toggler.length; i++) {
    toggler[i].addEventListener("click", function() {
        this.parentElement.querySelector(".nested").classList.toggle("active")
        this.classList.toggle("caret-down")
    })} 
}

function updateTreeViewFromKMeansData(results) {
    removeChildren(treeViewBase)
    var i = 0
    results.centroids.forEach(centroid => {
        const caret = document.createElement('li')
        treeViewBase.appendChild(caret)
        const span = document.createElement('span')
        caret.appendChild(span)
        span.classList.add('caret')
        span.textContent = 'Centroid ' + i
        i++
        const subList = document.createElement('ul')
        subList.classList.add('nested')
        caret.appendChild(subList)
        centroid.forEach(blog => {
            const subListItem = document.createElement('li')
            subList.appendChild(subListItem)
            subListItem.textContent = blog
        })
    })
    updateTreeViewTogglers()
}

var resultsTable = document.querySelector('#results-table')
document.querySelector('#button-k-means-clustering').addEventListener('click', button0)
document.querySelector('#button-hierarchical-clustering').addEventListener('click', button1)
var treeViewBase = document.querySelector('#myUL')
updateTreeViewTogglers()
