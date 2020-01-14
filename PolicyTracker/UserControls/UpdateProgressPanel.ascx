<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="UpdateProgressPanel.ascx.vb" Inherits="PolicyTracker.UpdateProgressPanel" %>
<div class="overlay">
</div>
<div class="UpdateProgress">
    <img src='<%= Page.ResolveUrl("~/Images/ajax-loader.gif")%>' alt="" style="vertical-align: bottom;" />
    &nbsp;Please Wait
</div>

<%--<div id="source-modal" class="modal fade">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h4 class="modal-title">Source Code</h4>
            </div>
            <div class="modal-body">
                <img src='<%= Page.ResolveUrl("~/Images/ajax-loader.gif")%>' alt="" style="vertical-align: bottom;" />&nbsp;Please Wait
            </div>
        </div>
    </div>
</div>--%>
<script type="text/javascript">
    //$("#source-modal").modal();
</script>
