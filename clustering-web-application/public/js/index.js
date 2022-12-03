/**
 * OnClick function for the K-Means Clustering button.
 */
async function button0() {
    setSpinnerAndListVisibility(false)
    const responseJSON = await fetchGetRequest("./KMeansClustering")
    updateTreeViewFromKMeansData(responseJSON)
    setSpinnerAndListVisibility(true)
}

/**
 * OnClick function for the Hierarchical Clustering button.
 */
async function button1() {
    setSpinnerAndListVisibility(false)
    const responseJSON = await fetchGetRequest("./HierarchicalClustering")
    updateTreeViewFromHierarchicalData(responseJSON)
    setSpinnerAndListVisibility(true)
}

/**
 * Makes a K-Means Clustering request to the Clustering API and returns the result JSON.
 *
 * @param {bool} setListToVisible - Whether to set the list to visible and spinner to hidden or vice versa.
 */
function setSpinnerAndListVisibility(setListToVisible) {
    if (setListToVisible) {
        spinnerContainer.setAttribute('hidden', 'true')
        listContainer.removeAttribute('hidden')
    } else {
        spinnerContainer.removeAttribute('hidden')
        listContainer.setAttribute('hidden', 'true')
    }
}

/**
 * Makes a GET request to the provided URL and returns the response as JSON.
 * 
 * @param {string} url - The url to make the request to.
 */
async function fetchGetRequest (url) {
    const response = await fetch(url, {
        method: 'get',
        headers: { 'Content-Type': 'application/json' }
    })
    return await response.json()
}

/**
 * Removes all children of a DOM node.
 * 
 * @param {Element} node - The node to remove all children from.
 */
function removeChildren(node) {
    while (node.lastChild !== null) {
        node.removeChild(node.lastChild)
    }
}

/**
 * Assigns OnClick functions to all the current Tree View togglers.
 */
function updateTreeViewTogglers() {
    var toggler = document.getElementsByClassName("caret");
    var i;

    for (i = 0; i < toggler.length; i++) {
    toggler[i].addEventListener("click", function() {
        this.parentElement.querySelector(".nested").classList.toggle("active")
        this.classList.toggle("caret-down")
    })} 
}

/**
 * Updates the Tree View to display a tree generated from a K-Means Clustering response from the API.
 * 
 * @param {Object} results - A K-Means Clustering response from the API.
 */
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

/**
 * Updates the Tree View to display a tree generated from a Hierarchical Clustering response from the API.
 * 
 * @param {Object} results - A Hierarchical Clustering response from the API.
 */
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

/**
 * Sets the state of all current Tree View togglers to expanded.
 */
function expandAll() {
    var toggler = document.getElementsByClassName("caret");
    var i;

    for (i = 0; i < toggler.length; i++) {
        toggler[i].parentElement.querySelector(".nested").classList.add("active")
        toggler[i].classList.add("caret-down")
    }
}

/**
 * Sets the state of all current Tree View togglers to collapsed.
 */
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
