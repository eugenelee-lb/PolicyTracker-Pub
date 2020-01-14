/*<![CDATA[*/

// prevent ValidationSummary cause page jump to top
window.scrollTo = function (x, y) {
    return true;
}
// Prevent Submit on Enter key is pressed
function PreventSubmitOnEnter() {
    if (event.keyCode == 13) {
        return false;
    }
}

// Places the focus on the first editable field in a form 
function placeFocus() {
    if (document.forms.length > 0) {
        var form1 = document.forms[0];
        for (i = 0; i < form1.length; i++) {
            if ((form1.elements[i].type == "text") || (form1.elements[i].type == "textarea") || (form1.elements[i].type.toString().charAt(0) == "s")) {
                form1.elements[i].focus();
                break;
            }
        }
    }
}

// Toggle Check Boxes's check state
function CheckBoxToggle(CheckBoxName, Checked) {
    if (document.forms.length > 0) {
        var form1 = document.forms[0];
        for (i = 0; i < form1.length; i++) {
            if ((form1.elements[i].type == "checkbox") && (form1.elements[i].id.indexOf(CheckBoxName) >= 0)) {
                form1.elements[i].checked = Checked;
            }
        }
    }
}

function ConfirmMultiDeletion(CheckBoxName) {
    var checkedCount = 0;
    if (document.forms.length > 0) {
        var form1 = document.forms[0];
        for (i = 0; i < form1.length; i++) {
            if ((form1.elements[i].type == "checkbox") && (form1.elements[i].id.indexOf(CheckBoxName) >= 0)) {
                if (form1.elements[i].checked) { checkedCount++ };
            }
        }
    }
    if (checkedCount == 0) {
        window.alert("Please select records to delete using check boxes.");
        return false;
    }
    else {
        return confirm("Are you sure you want to delete the selected " + checkedCount + " record(s)?");
    }
}

// This is a simple but effective script for extending your textarea with a maxlength attribute, 
// so the user's input cannot exceed a certain number of characters. Works with any number of textareas on the page.
function ismaxlength(obj, mlength) {
    //var mlength = obj.getAttribute ? parseInt(obj.getAttribute("maxlength")) : "";
    //if (obj.getAttribute && obj.value.length > mlength)
    if (obj.value.length > mlength) {
        obj.value = obj.value.substring(0, mlength);
        window.alert("Max length for this field is " + mlength + ".");
    }
}

/*]]>*/
