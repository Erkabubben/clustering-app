async function button0() {
    setSpinnerAndListVisibility(false)
    const responseJSON = await fetchGetRequest("./KMeansClustering")
    updateTreeViewFromKMeansData(responseJSON)
    setSpinnerAndListVisibility(true)
}

async function button1() {
    setSpinnerAndListVisibility(false)
    const responseJSON = await fetchGetRequest("./HierarchicalClustering")
    updateTreeViewFromHierarchicalData(responseJSON)
    setSpinnerAndListVisibility(true)
}

function setSpinnerAndListVisibility(listIsVisible) {
    if (listIsVisible) {
        spinnerContainer.setAttribute('hidden', 'true')
        listContainer.removeAttribute('hidden')
    } else {
        spinnerContainer.removeAttribute('hidden')
        listContainer.setAttribute('hidden', 'true')
    }
}

async function fetchGetRequest (url) {
    const response = await fetch(url, {
        method: 'get',
        headers: { 'Content-Type': 'application/json' }
    })
    return await response.json()
}

function removeChildren(node) {
    while (node.lastChild !== null) {
        node.removeChild(node.lastChild)
    }
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
    expandAll()
}

function updateTreeViewFromHierarchicalData(results)
{
    function iterateClusters(cluster_id, node) {
        const cluster = results.clusters[cluster_id]
        const caret = document.createElement('li')
        node.appendChild(caret)
        const span = document.createElement('span')
        caret.appendChild(span)
        span.classList.add('caret')
        const subList = document.createElement('ul')
        subList.classList.add('nested')
        caret.appendChild(subList)
        if (cluster.blog !== '') {
            const subListItem = document.createElement('li')
            subList.appendChild(subListItem)
            subListItem.textContent = cluster.blog
        }
        if (cluster.left !== -1) {
            iterateClusters(cluster.left, subList)
        }
        if (cluster.right !== -1) {
            iterateClusters(cluster.right, subList)
        }
    }

    removeChildren(treeViewBase)
    iterateClusters(0, treeViewBase)
    updateTreeViewTogglers()
    expandAll();
}


function expandAll() {
    var toggler = document.getElementsByClassName("caret");
    var i;

    for (i = 0; i < toggler.length; i++) {
        toggler[i].parentElement.querySelector(".nested").classList.add("active")
        toggler[i].classList.add("caret-down")
    }
}

function collapseAll() {
    var toggler = document.getElementsByClassName("caret");
    var i;

    for (i = 0; i < toggler.length; i++) {
        toggler[i].parentElement.querySelector(".nested").classList.remove("active")
        toggler[i].classList.remove("caret-down")
    }
}

document.querySelector('#button-k-means-clustering').addEventListener('click', button0)
document.querySelector('#button-hierarchical-clustering').addEventListener('click', button1)
document.querySelector('#button-expand').addEventListener('click', expandAll)
document.querySelector('#button-collapse').addEventListener('click', collapseAll)

var treeViewBase = document.querySelector('#myUL')
var spinnerContainer = document.querySelector('#spinner-container')
var listContainer = document.querySelector('#list-container')
spinnerContainer.setAttribute('hidden', 'true')
listContainer.setAttribute('hidden', 'true')
removeChildren(treeViewBase)
updateTreeViewTogglers()
