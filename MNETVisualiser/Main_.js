/**
 * Created by GVOL on 12.02.2015.
 */

var graph = Viva.Graph.graph();

var graphics = Viva.Graph.View.svgGraphics(),
    nodeSize = 50;

//Описание формы нодов
graphics.node(function(node) {
    var color = '#6600ff';
    //alert(node.id);
    if (node.id == 'Алиса') {
        color = '#ff0033';
    }
    var ui = Viva.Graph.svg('g')/*.attr('filter','url(#MyF)');
    //создание фильтра
    var filter = Viva.Graph.svg('filter')
        .attr('id','MyF');
    //Описание стадий фильтра
    var blurr = Viva.Graph.svg('feGaussianBlur')
        .attr('in', 'SourceAlpha')
        .attr('stdDeviation','4')
        .attr('result','blur');
    var оffset = Viva.Graph.svg('feOffset')
        .attr('in', 'blur')
        .attr('dx','4')
        .attr('dy','4')
        .attr('result','offsetBlur');
    var light = Viva.Graph.svg('feSpecularLighting')
        .attr('in', 'blur')
        .attr('surfaceScale','5')
        .attr('specularConstant','.75')
        .attr('specularExponent','20')
        .attr('lighting-color','#bbbbbb')
        .attr('result','specOut');
    var lpoint = Viva.Graph.svg('fePointLight')
        .attr('x','-5000')
        .attr('y','-10000')
        .attr('z','20000');
    light.append(lpoint);
    var fcomp1 = Viva.Graph.svg('feComposite')
        .attr('in','specOut')
        .attr('in2','SourceAlpha')
        .attr('operator','in')
        .attr('result','specOut');
    var fcomp2 = Viva.Graph.svg('feComposite')
        .attr('in','SourceGraphic')
        .attr('in2','specOut')
        .attr('operator','arithmetic')
        .attr('k1','0')
        .attr('k2','1')
        .attr('k3','1')
        .attr('k4','0')
        .attr('result','litPaint');
    var fmerge = Viva.Graph.svg('feMerge');
    var fm1 = Viva.Graph.svg('feMergeNode').attr('in','offsetBlur');
    var fm2 = Viva.Graph.svg('feMergeNode').attr('in','litPaint');
    fmerge.append(fm1);
    fmerge.append(fm2);

    filter.append(blurr);
    filter.append(оffset);
    filter.append(light);
    filter.append(fcomp1);
    filter.append(fcomp2);
    filter.append(fmerge);

     ui.append (filter);
    */
    var elepse = Viva.Graph.svg('ellipse')
        .attr('rx',nodeSize)
        .attr('ry',nodeSize/2)
        .attr('fill', color);
    var stext = Viva.Graph.svg('text')
                .attr('x', '-45px')
                .attr('y', '+3px')
                .text(node.data);

    ui.append (elepse);
    ui.append (stext);
    return ui;
        //.attr('width', nodeSize)
        //.attr('height', nodeSize)
        //.link('https://secure.gravatar.com/avatar/' + node.data);
}).placeNode(function(nodeUI, pos) {
    //nodeUI.attr('cx', pos.x).attr('cy', pos.y);
    nodeUI.attr('transform',
        'translate(' +
        (pos.x ) + ',' + (pos.y) +
        ')');
});


// To render an arrow we have to address two problems:
//  1. Links should start/stop at node's bounding box, not at the node center.
//  2. Render an arrow shape at the end of the link.

// Rendering arrow shape is achieved by using SVG markers, part of the SVG
// standard: http://www.w3.org/TR/SVG/painting.html#Markers
var createMarker = function(id) {
        return Viva.Graph.svg('marker')
            .attr('id', id)
            .attr('viewBox', "0 0 10 10")
            .attr('refX', "10")
            .attr('refY', "5")
            .attr('markerUnits', "strokeWidth")
            .attr('markerWidth', "10")
            .attr('markerHeight', "5")
            .attr('orient', "auto");
    },

    marker = createMarker('Triangle');
marker.append('path').attr('d', 'M 0 0 L 10 5 L 0 10 z');

// Marker should be defined only once in <defs> child element of root <svg> element:
var defs = graphics.getSvgRoot().append('defs');
defs.append(marker);

var geom = Viva.Graph.geom();

graphics.link(function(link){
    // Notice the Triangle marker-end attribe:
    return Viva.Graph.svg('path')
        .attr('stroke', 'gray')
        .attr('marker-end', 'url(#Triangle)');
}).placeLink(function(linkUI, fromPos, toPos) {
    // Here we should take care about
    //  "Links should start/stop at node's bounding box, not at the node center."

    // For rectangular nodes Viva.Graph.geom() provides efficient way to find
    // an intersection point between segment and rectangle
    var toNodeSize = nodeSize,
        fromNodeSize = nodeSize;

    var from = geom.intersectRect(
            // rectangle:
            fromPos.x - fromNodeSize / 2, // left
            fromPos.y - fromNodeSize / 2, // top
            fromPos.x + fromNodeSize / 2, // right
            fromPos.y + fromNodeSize / 2, // bottom
            // segment:
            fromPos.x, fromPos.y, toPos.x, toPos.y)
        || fromPos; // if no intersection found - return center of the node

    var to = geom.intersectRect(
            // rectangle:
            toPos.x - toNodeSize / 1, // left
            toPos.y - toNodeSize / 2, // top
            toPos.x + toNodeSize / 1, // right
            toPos.y + toNodeSize / 2, // bottom
            // segment:
            toPos.x, toPos.y, fromPos.x, fromPos.y)
        || toPos; // if no intersection found - return center of the node

    var data = 'M' + from.x + ',' + from.y +
        'L' + to.x + ',' + to.y;

    linkUI.attr("d", data);
});

// Finally we add something to the graph:

/***REPLACE***/

var layout = Viva.Graph.Layout.forceDirected(graph, {
        springLength : 100,
        springCoeff : 0.00001,
        dragCoeff : 0.01,
        theta : 0.8,
        gravity : -5.2
});

// All is ready. Render the graph:
var renderer = Viva.Graph.View.renderer(graph, {
    layout     : layout,
    graphics : graphics
});
renderer.run();
